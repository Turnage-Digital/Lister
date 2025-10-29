using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Domain.Services;

public static class MigrationPlanBuilder
{
    public static MigrationPlan Build(
        IEnumerable<Column> currentColumns,
        IEnumerable<Column> nextColumns,
        IEnumerable<Status> nextStatuses,
        IReadOnlyCollection<string> reasons
    )
    {
        var reasonSet = reasons?.ToHashSet(StringComparer.OrdinalIgnoreCase) ??
                        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var currentByName = currentColumns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
        var nextByName = nextColumns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        string KeyOf(Column column)
        {
            return string.IsNullOrWhiteSpace(column.StorageKey)
                ? column.Property
                : column.StorageKey!;
        }

        var changeColumnTypes = new Dictionary<string, ChangeColumnTypeOp>(StringComparer.OrdinalIgnoreCase);
        var removeColumns = new Dictionary<string, RemoveColumnOp>(StringComparer.OrdinalIgnoreCase);
        var tightenOps = new Dictionary<string, TightenConstraintsOp>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, currentColumn) in currentByName)
        {
            if (!nextByName.TryGetValue(name, out var nextColumn))
            {
                if (reasonSet.Any(r =>
                        r.Contains(name, StringComparison.OrdinalIgnoreCase) &&
                        r.Contains("remove", StringComparison.OrdinalIgnoreCase)))
                {
                    var removedKey = KeyOf(currentColumn);
                    removeColumns[removedKey] = new RemoveColumnOp(removedKey, "drop");
                }

                continue;
            }

            var storageKey = KeyOf(currentColumn);

            if (!Equals(currentColumn.Type, nextColumn.Type) && reasonSet.Any(r =>
                    r.Contains(name, StringComparison.OrdinalIgnoreCase) &&
                    r.Contains("type", StringComparison.OrdinalIgnoreCase)))
            {
                changeColumnTypes[storageKey] = new ChangeColumnTypeOp(storageKey, nextColumn.Type, "auto");
            }

            if (!currentColumn.Required && nextColumn.Required && reasonSet.Any(r =>
                    r.Contains(name, StringComparison.OrdinalIgnoreCase) &&
                    r.Contains("required", StringComparison.OrdinalIgnoreCase)))
            {
                MergeTighten(storageKey, true);
            }

            if (currentColumn.AllowedValues is { Length: > 0 } && nextColumn.AllowedValues is { Length: > 0 }
                                                               && !nextColumn.AllowedValues.All(v =>
                                                                   currentColumn.AllowedValues.Contains(v,
                                                                       StringComparer.OrdinalIgnoreCase))
                                                               && reasonSet.Any(r =>
                                                                   r.Contains(name,
                                                                       StringComparison.OrdinalIgnoreCase) &&
                                                                   r.Contains("allowedvalues",
                                                                       StringComparison.OrdinalIgnoreCase)))
            {
                MergeTighten(storageKey, values: nextColumn.AllowedValues);
            }

            if (currentColumn.MinNumber is not null && nextColumn.MinNumber is not null &&
                nextColumn.MinNumber > currentColumn.MinNumber && reasonSet.Any(r =>
                    r.Contains(name, StringComparison.OrdinalIgnoreCase) &&
                    r.Contains("minimum", StringComparison.OrdinalIgnoreCase)))
            {
                MergeTighten(storageKey, min: nextColumn.MinNumber);
            }

            if (currentColumn.MaxNumber is not null && nextColumn.MaxNumber is not null &&
                nextColumn.MaxNumber < currentColumn.MaxNumber && reasonSet.Any(r =>
                    r.Contains(name, StringComparison.OrdinalIgnoreCase) &&
                    r.Contains("maximum", StringComparison.OrdinalIgnoreCase)))
            {
                MergeTighten(storageKey, max: nextColumn.MaxNumber);
            }

            if (!string.Equals(currentColumn.Regex, nextColumn.Regex, StringComparison.Ordinal)
                && reasonSet.Any(r =>
                    r.Contains(name, StringComparison.OrdinalIgnoreCase) &&
                    r.Contains("regex", StringComparison.OrdinalIgnoreCase)))
            {
                MergeTighten(storageKey, regex: nextColumn.Regex);
            }
        }

        var nextStatusNames = nextStatuses.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var removeStatuses = reasonSet
            .Where(r => r.Contains("status", StringComparison.OrdinalIgnoreCase))
            .SelectMany(r => nextStatusNames.Where(name => r.Contains(name, StringComparison.OrdinalIgnoreCase)))
            .Select(name => new RemoveStatusOp(name, null))
            .ToArray();

        return new MigrationPlan
        {
            ChangeColumnTypes = changeColumnTypes.Count > 0 ? changeColumnTypes.Values.ToArray() : null,
            RemoveColumns = removeColumns.Count > 0 ? removeColumns.Values.ToArray() : null,
            TightenConstraints = tightenOps.Count > 0 ? tightenOps.Values.ToArray() : null,
            RemoveStatuses = removeStatuses.Length > 0 ? removeStatuses : null
        };

        void MergeTighten(
            string storageKey,
            bool? required = null,
            string[]? values = null,
            decimal? min = null,
            decimal? max = null,
            string? regex = null
        )
        {
            if (!tightenOps.TryGetValue(storageKey, out var existingOp))
            {
                existingOp = new TightenConstraintsOp(storageKey, null, null, null, null, null);
            }

            tightenOps[storageKey] = new TightenConstraintsOp(
                storageKey,
                required ?? existingOp.Required,
                values ?? existingOp.AllowedValues,
                min ?? existingOp.MinNumber,
                max ?? existingOp.MaxNumber,
                regex ?? existingOp.Regex
            );
        }
    }
}