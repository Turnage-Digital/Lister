using Lister.Lists.Application.Endpoints.Commands.Migrations;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.ReadOnly.Queries;

namespace Lister.Lists.Application.Migrations.Services;

public interface IMigrationValidator
{
    Task<MigrationResult> ValidateAsync(Guid listId, MigrationPlan plan, CancellationToken ct);
}

public class MigrationValidator(IGetListItemDefinition getter) : IMigrationValidator
{
    public async Task<MigrationResult> ValidateAsync(Guid listId, MigrationPlan plan, CancellationToken ct)
    {
        var messages = new List<string>();
        var def = await getter.GetAsync(listId, ct)
                  ?? throw new InvalidOperationException("List not found");

        var byKey = def.Columns.ToDictionary(
            c => string.IsNullOrWhiteSpace(c.StorageKey) ? c.Property : c.StorageKey!,
            c => c,
            StringComparer.OrdinalIgnoreCase);

        if (plan.RenameStorageKeys is { Length: > 0 })
        {
            foreach (var op in plan.RenameStorageKeys)
            {
                if (!byKey.ContainsKey(op.From))
                {
                    messages.Add($"RenameStorageKey: source '{op.From}' not found");
                }

                if (byKey.ContainsKey(op.To))
                {
                    messages.Add($"RenameStorageKey: destination '{op.To}' already exists");
                }
            }
        }

        if (plan.ChangeColumnTypes is { Length: > 0 })
        {
            foreach (var op in plan.ChangeColumnTypes)
            {
                if (!byKey.TryGetValue(op.Key, out var col))
                {
                    messages.Add($"ChangeColumnType: key '{op.Key}' not found");
                    continue;
                }

                if (col.Type == op.TargetType)
                {
                    messages.Add($"ChangeColumnType: key '{op.Key}' already has type {op.TargetType}");
                }

                if (string.IsNullOrWhiteSpace(op.Converter))
                {
                    messages.Add($"ChangeColumnType: key '{op.Key}' requires a converter");
                }
            }
        }

        if (plan.RemoveColumns is { Length: > 0 })
        {
            foreach (var op in plan.RemoveColumns)
            {
                if (!byKey.ContainsKey(op.Key))
                {
                    messages.Add($"RemoveColumn: key '{op.Key}' not found");
                }

                if (string.IsNullOrWhiteSpace(op.Policy))
                {
                    messages.Add($"RemoveColumn: key '{op.Key}' requires a policy");
                }
            }
        }

        if (plan.TightenConstraints is { Length: > 0 })
        {
            foreach (var op in plan.TightenConstraints)
            {
                if (!byKey.ContainsKey(op.Key))
                {
                    messages.Add($"TightenConstraints: key '{op.Key}' not found");
                }
            }
        }

        if (plan.RemoveStatuses is { Length: > 0 })
        {
            var statuses = def.Statuses.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var op in plan.RemoveStatuses)
            {
                if (!statuses.Contains(op.Name))
                {
                    messages.Add($"RemoveStatus: status '{op.Name}' not found");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(op.MapTo) && !statuses.Contains(op.MapTo))
                {
                    messages.Add($"RemoveStatus: mapping target '{op.MapTo}' not found");
                }
            }
        }

        return new MigrationResult
        {
            IsSafe = messages.Count == 0,
            Messages = messages.ToArray(),
            SuggestedPlan = plan
        };
    }
}