using System.Text.Json;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Migrations;

public class MigrationExecutor<TList, TItem>(
    IListsUnitOfWork<TList, TItem> unitOfWork,
    IMediator mediator
)
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<MigrationDryRunResult> ExecuteAsync(
        Guid listId,
        string userId,
        MigrationPlan plan,
        CancellationToken ct
    )
    {
        var correlationId = Guid.NewGuid();
        await mediator.Publish(new ListMigrationStartedIntegrationEvent(listId, correlationId, userId), ct);

        var list = await unitOfWork.ListsStore.GetByIdAsync(listId, ct) ??
                   throw new InvalidOperationException("List not found");

        var processed = 0;

        // Load current definition
        var columns = (await unitOfWork.ListsStore.GetColumnsAsync(list, ct)).ToList();
        var statuses = (await unitOfWork.ListsStore.GetStatusesAsync(list, ct)).ToList();

        // Helper to resolve by storage key with property fallback
        string KeyOf(Column c)
        {
            return string.IsNullOrWhiteSpace(c.StorageKey) ? c.Property : c.StorageKey!;
        }

        // Rename storage keys (metadata only)
        if (plan.RenameStorageKeys is { Length: > 0 })
        {
            foreach (var op in plan.RenameStorageKeys)
            {
                var col = columns.FirstOrDefault(c =>
                    string.Equals(KeyOf(c), op.From, StringComparison.OrdinalIgnoreCase));
                if (col != null)
                {
                    col.StorageKey = op.To;
                }
            }

            await unitOfWork.ListsStore.SetColumnsAsync(list, columns, userId, ct);
            await mediator.Publish(new ListMigrationProgressIntegrationEvent(listId, correlationId,
                $"Renamed {plan.RenameStorageKeys.Length} storage keys", 10), ct);
        }

        // Change column types (convert bags)
        if (plan.ChangeColumnTypes is { Length: > 0 })
        {
            foreach (var op in plan.ChangeColumnTypes)
            {
                var col = columns.FirstOrDefault(c =>
                    string.Equals(KeyOf(c), op.Key, StringComparison.OrdinalIgnoreCase));
                if (col == null)
                {
                    continue;
                }

                // update metadata
                col.Type = op.TargetType;

                // per-item conversion
                var itemIds = await unitOfWork.ItemsStore.GetItemIdsAsync(listId, ct);
                var total = itemIds.Length;
                var done = 0;
                foreach (var id in itemIds)
                {
                    var item = await unitOfWork.ItemsStore.GetByIdAsync(id, listId, ct);
                    if (item == null)
                    {
                        continue;
                    }

                    var bagObj = await unitOfWork.ItemsStore.GetBagAsync(item, ct);
                    var dict = ToDictionary(bagObj);
                    var key = col.StorageKey ?? col.Property;
                    if (dict.TryGetValue(key, out var value))
                    {
                        dict[key] = ConvertValue(value, op.TargetType, op.Converter);
                        await unitOfWork.ItemsStore.SetBagAsync(item, dict, userId, ct);
                    }

                    done++;
                    if (done % 50 == 0)
                    {
                        var percent = 10 + (int)Math.Round(70.0 * done / Math.Max(1, total));
                        await mediator.Publish(new ListMigrationProgressIntegrationEvent(listId, correlationId,
                            $"Converting '{op.Key}' {done}/{total}", percent), ct);
                    }
                }

                processed += total;
            }

            await unitOfWork.ListsStore.SetColumnsAsync(list, columns, userId, ct);
        }

        // Remove columns (drop from metadata and bags)
        if (plan.RemoveColumns is { Length: > 0 })
        {
            var removeSet = plan.RemoveColumns.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var itemIds = await unitOfWork.ItemsStore.GetItemIdsAsync(listId, ct);
            var total = itemIds.Length;
            var done = 0;
            foreach (var id in itemIds)
            {
                var item = await unitOfWork.ItemsStore.GetByIdAsync(id, listId, ct);
                if (item == null)
                {
                    continue;
                }

                var bagObj = await unitOfWork.ItemsStore.GetBagAsync(item, ct);
                var dict = ToDictionary(bagObj);
                foreach (var r in removeSet)
                {
                    dict.Remove(r);
                }

                await unitOfWork.ItemsStore.SetBagAsync(item, dict, userId, ct);
                done++;
                if (done % 50 == 0)
                {
                    var pct = 80 + (int)Math.Round(10.0 * done / Math.Max(1, total));
                    await mediator.Publish(new ListMigrationProgressIntegrationEvent(listId, correlationId,
                        $"Removing columns {done}/{total}", pct), ct);
                }
            }

            processed += total;
            columns = columns.Where(c => !removeSet.Contains(KeyOf(c))).ToList();
            await unitOfWork.ListsStore.SetColumnsAsync(list, columns, userId, ct);
        }

        // Tighten constraints: metadata only (per earlier guardrails)
        if (plan.TightenConstraints is { Length: > 0 })
        {
            foreach (var op in plan.TightenConstraints)
            {
                var col = columns.FirstOrDefault(c =>
                    string.Equals(KeyOf(c), op.Key, StringComparison.OrdinalIgnoreCase));
                if (col == null)
                {
                    continue;
                }

                if (op.Required is not null)
                {
                    col.Required = op.Required.Value;
                }

                if (op.AllowedValues is not null)
                {
                    col.AllowedValues = op.AllowedValues;
                }

                if (op.MinNumber is not null)
                {
                    col.MinNumber = op.MinNumber;
                }

                if (op.MaxNumber is not null)
                {
                    col.MaxNumber = op.MaxNumber;
                }

                if (op.Regex is not null)
                {
                    col.Regex = op.Regex;
                }
            }

            await unitOfWork.ListsStore.SetColumnsAsync(list, columns, userId, ct);
        }

        // Remove statuses with mapping
        if (plan.RemoveStatuses is { Length: > 0 })
        {
            var removed = plan.RemoveStatuses.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            statuses = statuses.Where(s => !removed.Contains(s.Name)).ToList();
            var mapping = plan.RemoveStatuses.Where(s => !string.IsNullOrWhiteSpace(s.MapTo))
                .ToDictionary(s => s.Name, s => s.MapTo!, StringComparer.OrdinalIgnoreCase);

            var itemIds = await unitOfWork.ItemsStore.GetItemIdsAsync(listId, ct);
            var total = itemIds.Length;
            var done = 0;
            foreach (var id in itemIds)
            {
                var item = await unitOfWork.ItemsStore.GetByIdAsync(id, listId, ct);
                if (item == null)
                {
                    continue;
                }

                var bagObj = await unitOfWork.ItemsStore.GetBagAsync(item, ct);
                var dict = ToDictionary(bagObj);
                if (dict.TryGetValue("status", out var current) && current is string cs &&
                    mapping.TryGetValue(cs, out var mt))
                {
                    dict["status"] = mt;
                    await unitOfWork.ItemsStore.SetBagAsync(item, dict, userId, ct);
                }

                done++;
                if (done % 50 == 0)
                {
                    var pct = 90 + (int)Math.Round(10.0 * done / Math.Max(1, total));
                    await mediator.Publish(new ListMigrationProgressIntegrationEvent(listId, correlationId,
                        $"Mapping statuses {done}/{total}", pct), ct);
                }
            }

            await unitOfWork.ListsStore.SetStatusesAsync(list, statuses, userId, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
        await mediator.Publish(new ListMigrationCompletedIntegrationEvent(listId, correlationId, userId, processed),
            ct);

        return new MigrationDryRunResult { IsSafe = true, Messages = [] };
    }

    private static Dictionary<string, object?> ToDictionary(object bag)
    {
        if (bag is Dictionary<string, object?> d)
        {
            return new Dictionary<string, object?>(d, StringComparer.OrdinalIgnoreCase);
        }

        var json = JsonSerializer.Serialize(bag);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(json) ?? new Dictionary<string, object?>();
        return new Dictionary<string, object?>(dict, StringComparer.OrdinalIgnoreCase);
    }

    private static object? ConvertValue(object? value, ColumnType targetType, string converter)
    {
        if (value is null)
        {
            return null;
        }

        try
        {
            return targetType switch
            {
                ColumnType.Text => value.ToString(),
                ColumnType.Number => TryToDecimal(value, converter),
                ColumnType.Boolean => TryToBool(value, converter),
                ColumnType.Date => TryToDate(value, converter),
                _ => value
            };
        }
        catch
        {
            return value; // leave as-is on conversion error
        }
    }

    private static object? TryToDecimal(object v, string converter)
    {
        if (v is decimal or int or long or double or float)
        {
            return Convert.ToDecimal(v);
        }

        if (v is string s && decimal.TryParse(s, out var d))
        {
            return d;
        }

        return v;
    }

    private static object? TryToBool(object v, string converter)
    {
        if (v is bool b)
        {
            return b;
        }

        if (v is string s && bool.TryParse(s, out var rb))
        {
            return rb;
        }

        return v;
    }

    private static object? TryToDate(object v, string converter)
    {
        if (v is DateTime dt)
        {
            return dt;
        }

        if (v is string s && DateTime.TryParse(s, out var d))
        {
            return d;
        }

        return v;
    }
}