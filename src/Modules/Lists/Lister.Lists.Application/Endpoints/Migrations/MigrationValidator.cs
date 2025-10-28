using Lister.Lists.ReadOnly.Queries;

namespace Lister.Lists.Application.Endpoints.Migrations;

public interface IMigrationValidator
{
    Task<MigrationDryRunResult> ValidateAsync(Guid listId, MigrationPlan plan, CancellationToken ct);
}

public class MigrationValidator(IGetListItemDefinition getter) : IMigrationValidator
{
    public async Task<MigrationDryRunResult> ValidateAsync(Guid listId, MigrationPlan plan, CancellationToken ct)
    {
        var messages = new List<string>();
        var def = await getter.GetAsync(listId, ct) ?? throw new InvalidOperationException("List not found");

        // Build lookup by storage key with fallback to property
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
                if (!byKey.TryGetValue(op.Key, out var col))
                {
                    messages.Add($"TightenConstraints: key '{op.Key}' not found");
                    continue;
                }

                if (op.Required is true && col.Required)
                {
                    messages.Add($"TightenConstraints: key '{op.Key}' already required");
                }

                // allowed values tightening is safe only if superset check holds for current → proposed
                if (op.AllowedValues is { Length: > 0 } && col.AllowedValues is { Length: > 0 })
                {
                    var cur = new HashSet<string>(col.AllowedValues, StringComparer.OrdinalIgnoreCase);
                    if (!op.AllowedValues.All(v => cur.Contains(v)))
                    {
                        messages.Add($"TightenConstraints: key '{op.Key}' allowedValues must be superset of current");
                    }
                }

                if (op.MinNumber is not null && col.MinNumber is not null && op.MinNumber > col.MinNumber)
                {
                    // This is a breaking tighten; flag it for migration executor handling
                    messages.Add($"TightenConstraints: key '{op.Key}' increases minimum");
                }

                if (op.MaxNumber is not null && col.MaxNumber is not null && op.MaxNumber < col.MaxNumber)
                {
                    messages.Add($"TightenConstraints: key '{op.Key}' decreases maximum");
                }

                if (!string.IsNullOrWhiteSpace(op.Regex) && !string.Equals(op.Regex, col.Regex))
                {
                    messages.Add($"TightenConstraints: key '{op.Key}' changes regex");
                }
            }
        }

        if (plan.RemoveStatuses is { Length: > 0 })
        {
            var names = new HashSet<string>(def.Statuses.Select(s => s.Name), StringComparer.OrdinalIgnoreCase);
            foreach (var op in plan.RemoveStatuses)
            {
                if (!names.Contains(op.Name))
                {
                    messages.Add($"RemoveStatus: '{op.Name}' not found");
                }

                if (!string.IsNullOrWhiteSpace(op.MapTo) && !names.Contains(op.MapTo))
                {
                    messages.Add($"RemoveStatus: mapping target '{op.MapTo}' not found");
                }
            }
        }

        return new MigrationDryRunResult
        {
            IsSafe = messages.Count == 0,
            Messages = messages.ToArray()
        };
    }
}