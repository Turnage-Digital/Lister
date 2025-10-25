using System.Text.Json;
using Lister.Lists.Application.Endpoints.Migrations.Shared;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Endpoints.Migrations.RunMigration;

public class MigrationExecutor<TList, TItem, TMigrationJob>(
    IListsUnitOfWork<TList, TItem, TMigrationJob> unitOfWork,
    IGetListItemStream itemStream,
    IGetListItemCount itemCount
) where TList : IWritableList where TItem : IWritableItem where TMigrationJob : IWritableListMigrationJob
{
    private const int BatchSize = 200;
    private readonly IGetListItemCount _itemCount = itemCount;
    private readonly IGetListItemStream _itemStream = itemStream;

    public async Task<MigrationExecutionResult> ExecuteAsync(
        Guid listId,
        string userId,
        MigrationPlan plan,
        Func<MigrationProgressSnapshot, CancellationToken, Task> progressCallback,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(progressCallback);

        cancellationToken.ThrowIfCancellationRequested();

        var list = await unitOfWork.ListsStore.GetByIdAsync(listId, cancellationToken)
                   ?? throw new InvalidOperationException("List not found");

        var columns = (await unitOfWork.ListsStore.GetColumnsAsync(list, cancellationToken)).ToList();
        var statuses = (await unitOfWork.ListsStore.GetStatusesAsync(list, cancellationToken)).ToList();

        var itemCount = await _itemCount.CountAsync(listId, cancellationToken);
        var totalPasses = CountItemPasses(plan);
        var jobTotalItems = itemCount * totalPasses;
        var processedSoFar = 0;

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

            await unitOfWork.ListsStore.SetColumnsAsync(list, columns, userId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await progressCallback(
                new MigrationProgressSnapshot(
                    $"Renamed {plan.RenameStorageKeys.Length} storage keys",
                    10,
                    processedSoFar,
                    jobTotalItems),
                cancellationToken);
        }

        // Change column types (convert bags per column)
        if (plan.ChangeColumnTypes is { Length: > 0 })
        {
            foreach (var op in plan.ChangeColumnTypes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var col = columns.FirstOrDefault(c =>
                    string.Equals(KeyOf(c), op.Key, StringComparison.OrdinalIgnoreCase));
                if (col is null)
                {
                    continue;
                }

                col.Type = op.TargetType;

                var processedForColumn = await ConvertColumnTypeAsync(
                    listId,
                    userId,
                    col,
                    op,
                    itemCount,
                    processedSoFar,
                    jobTotalItems,
                    progressCallback,
                    cancellationToken);

                processedSoFar += processedForColumn;
            }

            await unitOfWork.ListsStore.SetColumnsAsync(list, columns, userId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Remove columns (metadata + bag updates)
        if (plan.RemoveColumns is { Length: > 0 })
        {
            var removeSet = plan.RemoveColumns.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var processedForRemoval = await RemoveColumnsAsync(
                listId,
                userId,
                removeSet,
                itemCount,
                processedSoFar,
                jobTotalItems,
                progressCallback,
                cancellationToken);

            processedSoFar += processedForRemoval;
            columns = columns.Where(c => !removeSet.Contains(KeyOf(c))).ToList();
            await unitOfWork.ListsStore.SetColumnsAsync(list, columns, userId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Tighten constraints: metadata only
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

            await unitOfWork.ListsStore.SetColumnsAsync(list, columns, userId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await progressCallback(
                new MigrationProgressSnapshot(
                    "Updated column constraints",
                    85,
                    processedSoFar,
                    jobTotalItems),
                cancellationToken);
        }

        // Remove statuses with mapping
        if (plan.RemoveStatuses is { Length: > 0 })
        {
            var removed = plan.RemoveStatuses.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            statuses = statuses.Where(s => !removed.Contains(s.Name)).ToList();
            var mapping = plan.RemoveStatuses.Where(s => !string.IsNullOrWhiteSpace(s.MapTo))
                .ToDictionary(s => s.Name, s => s.MapTo!, StringComparer.OrdinalIgnoreCase);

            var processedForStatuses = await MapStatusesAsync(
                listId,
                userId,
                mapping,
                itemCount,
                processedSoFar,
                jobTotalItems,
                progressCallback,
                cancellationToken);

            processedSoFar += processedForStatuses;
            await unitOfWork.ListsStore.SetStatusesAsync(list, statuses, userId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new MigrationExecutionResult(processedSoFar, jobTotalItems);
    }

    private async Task<int> ConvertColumnTypeAsync(
        Guid listId,
        string userId,
        Column column,
        ChangeColumnTypeOp op,
        int itemCount,
        int processedBase,
        int jobTotalItems,
        Func<MigrationProgressSnapshot, CancellationToken, Task> progressCallback,
        CancellationToken ct
    )
    {
        if (itemCount == 0)
        {
            return 0;
        }

        var processed = 0;
        var batch = new List<int>(BatchSize);
        var itemIds = _itemStream.StreamAsync(listId, ct);

        await foreach (var itemId in itemIds)
        {
            ct.ThrowIfCancellationRequested();
            batch.Add(itemId);

            if (batch.Count >= BatchSize)
            {
                processed += await ConvertBatchAsync(batch, listId, userId, column, op, ct);
                await unitOfWork.SaveChangesAsync(ct);
                var percent = 10 + (int)Math.Round(70.0 * processed / Math.Max(1, itemCount));
                await progressCallback(
                    new MigrationProgressSnapshot(
                        $"Converting '{op.Key}' {processed}/{itemCount}",
                        percent,
                        processedBase + processed,
                        jobTotalItems),
                    ct);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            processed += await ConvertBatchAsync(batch, listId, userId, column, op, ct);
            await unitOfWork.SaveChangesAsync(ct);
            var percent = 10 + (int)Math.Round(70.0 * processed / Math.Max(1, itemCount));
            await progressCallback(
                new MigrationProgressSnapshot(
                    $"Converting '{op.Key}' {processed}/{itemCount}",
                    percent,
                    processedBase + processed,
                    jobTotalItems),
                ct);
            batch.Clear();
        }

        return processed;
    }

    private async Task<int> RemoveColumnsAsync(
        Guid listId,
        string userId,
        HashSet<string> removeSet,
        int itemCount,
        int processedBase,
        int jobTotalItems,
        Func<MigrationProgressSnapshot, CancellationToken, Task> progressCallback,
        CancellationToken ct
    )
    {
        if (itemCount == 0)
        {
            return 0;
        }

        var processed = 0;
        var batch = new List<int>(BatchSize);

        await foreach (var itemId in _itemStream.StreamAsync(listId, ct))
        {
            ct.ThrowIfCancellationRequested();
            batch.Add(itemId);

            if (batch.Count >= BatchSize)
            {
                processed += await RemoveColumnsBatchAsync(batch, listId, userId, removeSet, ct);
                await unitOfWork.SaveChangesAsync(ct);
                var percent = 80 + (int)Math.Round(10.0 * processed / Math.Max(1, itemCount));
                await progressCallback(
                    new MigrationProgressSnapshot(
                        $"Removing columns {processed}/{itemCount}",
                        percent,
                        processedBase + processed,
                        jobTotalItems),
                    ct);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            processed += await RemoveColumnsBatchAsync(batch, listId, userId, removeSet, ct);
            await unitOfWork.SaveChangesAsync(ct);
            var percent = 80 + (int)Math.Round(10.0 * processed / Math.Max(1, itemCount));
            await progressCallback(
                new MigrationProgressSnapshot(
                    $"Removing columns {processed}/{itemCount}",
                    percent,
                    processedBase + processed,
                    jobTotalItems),
                ct);
            batch.Clear();
        }

        return processed;
    }

    private async Task<int> MapStatusesAsync(
        Guid listId,
        string userId,
        IReadOnlyDictionary<string, string> mapping,
        int itemCount,
        int processedBase,
        int jobTotalItems,
        Func<MigrationProgressSnapshot, CancellationToken, Task> progressCallback,
        CancellationToken ct
    )
    {
        if (itemCount == 0 || mapping.Count == 0)
        {
            return 0;
        }

        var processed = 0;
        var batch = new List<int>(BatchSize);

        await foreach (var itemId in _itemStream.StreamAsync(listId, ct))
        {
            ct.ThrowIfCancellationRequested();
            batch.Add(itemId);

            if (batch.Count >= BatchSize)
            {
                processed += await MapStatusesBatchAsync(batch, listId, userId, mapping, ct);
                await unitOfWork.SaveChangesAsync(ct);
                var percent = 90 + (int)Math.Round(10.0 * processed / Math.Max(1, itemCount));
                await progressCallback(
                    new MigrationProgressSnapshot(
                        $"Mapping statuses {processed}/{itemCount}",
                        percent,
                        processedBase + processed,
                        jobTotalItems),
                    ct);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            processed += await MapStatusesBatchAsync(batch, listId, userId, mapping, ct);
            await unitOfWork.SaveChangesAsync(ct);
            var percent = 90 + (int)Math.Round(10.0 * processed / Math.Max(1, itemCount));
            await progressCallback(
                new MigrationProgressSnapshot(
                    $"Mapping statuses {processed}/{itemCount}",
                    percent,
                    processedBase + processed,
                    jobTotalItems),
                ct);
            batch.Clear();
        }

        return processed;
    }

    private async Task<int> ConvertBatchAsync(
        IReadOnlyCollection<int> itemIds,
        Guid listId,
        string userId,
        Column column,
        ChangeColumnTypeOp op,
        CancellationToken ct
    )
    {
        var processed = 0;
        foreach (var id in itemIds)
        {
            ct.ThrowIfCancellationRequested();
            var item = await unitOfWork.ItemsStore.GetByIdAsync(id, listId, ct);
            if (item == null)
            {
                continue;
            }

            var bagObj = await unitOfWork.ItemsStore.GetBagAsync(item, ct);
            var dict = ToDictionary(bagObj);
            var key = column.StorageKey ?? column.Property;
            if (dict.TryGetValue(key, out var value))
            {
                dict[key] = ConvertValue(value, op.TargetType, op.Converter);
                await unitOfWork.ItemsStore.SetBagAsync(item, dict, userId, ct);
            }

            processed++;
        }

        return processed;
    }

    private async Task<int> RemoveColumnsBatchAsync(
        IReadOnlyCollection<int> itemIds,
        Guid listId,
        string userId,
        HashSet<string> removeSet,
        CancellationToken ct
    )
    {
        var processed = 0;

        foreach (var id in itemIds)
        {
            ct.ThrowIfCancellationRequested();
            var item = await unitOfWork.ItemsStore.GetByIdAsync(id, listId, ct);
            if (item == null)
            {
                continue;
            }

            var bagObj = await unitOfWork.ItemsStore.GetBagAsync(item, ct);
            var dict = ToDictionary(bagObj);
            var removed = false;
            foreach (var key in removeSet)
            {
                if (dict.Remove(key))
                {
                    removed = true;
                }
            }

            if (removed)
            {
                await unitOfWork.ItemsStore.SetBagAsync(item, dict, userId, ct);
            }

            processed++;
        }

        return processed;
    }

    private async Task<int> MapStatusesBatchAsync(
        IReadOnlyCollection<int> itemIds,
        Guid listId,
        string userId,
        IReadOnlyDictionary<string, string> mapping,
        CancellationToken ct
    )
    {
        var processed = 0;

        foreach (var id in itemIds)
        {
            ct.ThrowIfCancellationRequested();
            var item = await unitOfWork.ItemsStore.GetByIdAsync(id, listId, ct);
            if (item == null)
            {
                continue;
            }

            var bagObj = await unitOfWork.ItemsStore.GetBagAsync(item, ct);
            var dict = ToDictionary(bagObj);
            if (dict.TryGetValue("status", out var current) && current is string statusValue &&
                mapping.TryGetValue(statusValue, out var target))
            {
                dict["status"] = target;
                await unitOfWork.ItemsStore.SetBagAsync(item, dict, userId, ct);
            }

            processed++;
        }

        return processed;
    }

    private static Dictionary<string, object?> ToDictionary(object bag)
    {
        if (bag is Dictionary<string, object?> dict)
        {
            return new Dictionary<string, object?>(dict, StringComparer.OrdinalIgnoreCase);
        }

        var json = JsonSerializer.Serialize(bag);
        var parsed = JsonSerializer
            .Deserialize<Dictionary<string, object?>>(json) ?? new Dictionary<string, object?>();
        return new Dictionary<string, object?>(parsed, StringComparer.OrdinalIgnoreCase);
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
            return value;
        }
    }

    private static object? TryToDecimal(object value, string converter)
    {
        if (value is decimal or int or long or double or float)
        {
            return Convert.ToDecimal(value);
        }

        if (value is string s && decimal.TryParse(s, out var d))
        {
            return d;
        }

        return value;
    }

    private static object? TryToBool(object value, string converter)
    {
        if (value is bool b)
        {
            return b;
        }

        if (value is string s && bool.TryParse(s, out var rb))
        {
            return rb;
        }

        return value;
    }

    private static object? TryToDate(object value, string converter)
    {
        if (value is DateTime dt)
        {
            return dt;
        }

        if (value is string s && DateTime.TryParse(s, out var parsed))
        {
            return parsed;
        }

        return value;
    }

    private static int CountItemPasses(MigrationPlan plan)
    {
        var passes = 0;
        if (plan.ChangeColumnTypes is { Length: > 0 })
        {
            passes += plan.ChangeColumnTypes.Length;
        }

        if (plan.RemoveColumns is { Length: > 0 })
        {
            passes += 1;
        }

        if (plan.RemoveStatuses is { Length: > 0 })
        {
            passes += 1;
        }

        return passes;
    }
}