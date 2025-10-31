using System.Dynamic;
using Lister.Core.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Events;
using Lister.Lists.Domain.Exceptions;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Domain;

public class ListsAggregate<TList, TItem>(
    IListsUnitOfWork<TList, TItem> unitOfWork,
    IDomainEventQueue events,
    IValidateListItemBag<TList> bagValidator
)
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<TList?> GetListByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.GetByIdAsync(id, cancellationToken);
        return retval;
    }

    public async Task<TList?> GetListByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.GetByNameAsync(name, cancellationToken);
        return retval;
    }

    public async Task<TList> CreateListAsync(
        string createdBy,
        string name,
        IEnumerable<Status> statuses,
        IEnumerable<Column> columns,
        IEnumerable<StatusTransition>? transitions = null,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await unitOfWork.ListsStore.InitAsync(name, createdBy, cancellationToken);

        var normalizedColumns = AssignStorageKeys(columns);
        await unitOfWork.ListsStore.SetColumnsAsync(retval, normalizedColumns, createdBy, cancellationToken);
        await unitOfWork.ListsStore.SetStatusesAsync(retval, statuses, createdBy, cancellationToken);

        if (transitions is not null)
        {
            await unitOfWork.ListsStore.SetStatusTransitionsAsync(retval, transitions, createdBy, cancellationToken);
        }

        await unitOfWork.ListsStore.CreateAsync(retval, cancellationToken);
        events.Enqueue(new ListCreatedEvent(retval, createdBy), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return retval;
    }

    private static List<Column> AssignStorageKeys(IEnumerable<Column> columns)
    {
        var retval = columns.Select(c => new Column
            {
                StorageKey = c.StorageKey,
                Name = c.Name,
                Regex = c.Regex,
                Required = c.Required,
                AllowedValues = c.AllowedValues,
                MinNumber = c.MinNumber,
                MaxNumber = c.MaxNumber,
                Type = c.Type
            })
            .ToList();

        // Generate sequential keys for any missing StorageKey: prop1, prop2, ...
        var used = new HashSet<string>(retval.Where(c => !string.IsNullOrWhiteSpace(c.StorageKey))
            .Select(c => c.StorageKey!)
            .Select(s => s.ToLowerInvariant()));
        var counter = 1;
        foreach (var col in retval.Where(col => string.IsNullOrWhiteSpace(col.StorageKey)))
        {
            string key;
            do
            {
                key = $"prop{counter++}";
            } while (used.Contains(key));

            col.StorageKey = key;
            used.Add(key);
        }

        return retval;
    }

    public async Task DeleteListAsync(TList list, string deletedBy, CancellationToken cancellationToken = default)
    {
        await unitOfWork.ListsStore.DeleteAsync(list, deletedBy, cancellationToken);
        events.Enqueue(new ListDeletedEvent(list, deletedBy), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateListAsync(
        TList list,
        IEnumerable<Column>? columns,
        IEnumerable<Status>? statuses,
        IEnumerable<StatusTransition>? transitions,
        string updatedBy,
        CancellationToken cancellationToken = default
    )
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        // Load current
        var currentColumns = await unitOfWork.ListsStore.GetColumnsAsync(list, cancellationToken);
        var currentStatuses = await unitOfWork.ListsStore.GetStatusesAsync(list, cancellationToken);

        string KeyOf(Column column)
        {
            return string.IsNullOrWhiteSpace(column.StorageKey)
                ? column.Property
                : column.StorageKey!;
        }

        var changeColumnTypes = new Dictionary<string, ChangeColumnTypeOp>(StringComparer.OrdinalIgnoreCase);
        var removeColumns = new Dictionary<string, RemoveColumnOp>(StringComparer.OrdinalIgnoreCase);
        var tightenBuilders = new Dictionary<string, TightenConstraintsBuilder>(StringComparer.OrdinalIgnoreCase);
        var removeStatuses = new Dictionary<string, RemoveStatusOp>(StringComparer.OrdinalIgnoreCase);
        List<Column>? requestedColumns = null;

        static TightenConstraintsBuilder GetTightenBuilder(
            string key,
            IDictionary<string, TightenConstraintsBuilder> builders
        )
        {
            if (!builders.TryGetValue(key, out var builder))
            {
                builder = new TightenConstraintsBuilder(key);
                builders[key] = builder;
            }

            return builder;
        }

        var migrationReasons = new List<string>();

        List<Column>? normalizedColumns = null;

        if (columns is not null)
        {
            var incomingColumns = columns.ToList();
            requestedColumns = incomingColumns;
            var currentByName = currentColumns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            var nextByName = incomingColumns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            // Removed columns
            var removed = currentByName.Keys.Where(k => !nextByName.ContainsKey(k)).ToArray();
            if (removed.Length > 0)
            {
                migrationReasons.Add(
                    $"Update would remove columns: {string.Join(", ", removed)}. Removing columns requires a migration.");

                foreach (var name in removed)
                {
                    if (!currentByName.TryGetValue(name, out var column))
                    {
                        continue;
                    }

                    var key = KeyOf(column);
                    removeColumns[key] = new RemoveColumnOp(key, "drop");
                }
            }

            // Type changes
            var typeChanges = currentByName
                .Where(kv => nextByName.TryGetValue(kv.Key, out var next) && next.Type != kv.Value.Type)
                .Select(kv => new { Column = kv.Key, From = kv.Value.Type, To = nextByName[kv.Key].Type })
                .ToArray();
            if (typeChanges.Length > 0)
            {
                foreach (var change in typeChanges)
                {
                    migrationReasons.Add(
                        $"Column '{change.Column}' type change from {change.From} to {change.To} requires a migration.");

                    if (currentByName.TryGetValue(change.Column, out var column) &&
                        nextByName.TryGetValue(change.Column, out var nextColumn))
                    {
                        var key = KeyOf(column);
                        if (!changeColumnTypes.ContainsKey(key))
                        {
                            changeColumnTypes[key] = new ChangeColumnTypeOp(key, nextColumn.Type, "auto");
                        }
                    }
                }
            }

            // Tightening constraints heuristics
            foreach (var (key, cur) in currentByName)
            {
                if (!nextByName.TryGetValue(key, out var next))
                {
                    continue;
                }

                // Required: false -> true is breaking
                if (!cur.Required && next.Required)
                {
                    migrationReasons.Add(
                        $"Column '{key}': switching from optional to required requires a migration.");

                    if (currentByName.TryGetValue(key, out var column))
                    {
                        var storageKey = KeyOf(column);
                        var builder = GetTightenBuilder(storageKey, tightenBuilders);
                        builder.Required = true;
                    }
                }

                // AllowedValues: ensure next is superset (if both non-null)
                if (cur.AllowedValues is { Length: > 0 } && next.AllowedValues is { Length: > 0 })
                {
                    var curSet = new HashSet<string>(cur.AllowedValues, StringComparer.OrdinalIgnoreCase);
                    if (!next.AllowedValues.All(v => curSet.Contains(v)))
                    {
                        migrationReasons.Add(
                            $"Column '{key}': shrinking allowedValues requires a migration.");

                        if (currentByName.TryGetValue(key, out var column))
                        {
                            var storageKey = KeyOf(column);
                            var builder = GetTightenBuilder(storageKey, tightenBuilders);
                            builder.AllowedValues = next.AllowedValues?.ToArray();
                        }
                    }
                }

                // Range tightening
                if (cur.MinNumber is not null && next.MinNumber is not null && next.MinNumber > cur.MinNumber)
                {
                    migrationReasons.Add(
                        $"Column '{key}': increasing the minimum value requires a migration.");

                    if (currentByName.TryGetValue(key, out var column))
                    {
                        var storageKey = KeyOf(column);
                        var builder = GetTightenBuilder(storageKey, tightenBuilders);
                        builder.MinNumber = next.MinNumber;
                    }
                }

                if (cur.MaxNumber is not null && next.MaxNumber is not null && next.MaxNumber < cur.MaxNumber)
                {
                    migrationReasons.Add(
                        $"Column '{key}': decreasing the maximum value requires a migration.");

                    if (currentByName.TryGetValue(key, out var column))
                    {
                        var storageKey = KeyOf(column);
                        var builder = GetTightenBuilder(storageKey, tightenBuilders);
                        builder.MaxNumber = next.MaxNumber;
                    }
                }

                // Regex: any change treated as breaking
                if (!string.Equals(cur.Regex, next.Regex, StringComparison.Ordinal))
                {
                    migrationReasons.Add(
                        $"Column '{key}': regex changes require a migration.");

                    if (currentByName.TryGetValue(key, out var column))
                    {
                        var storageKey = KeyOf(column);
                        var builder = GetTightenBuilder(storageKey, tightenBuilders);
                        builder.Regex = next.Regex;
                    }
                }
            }

            if (!migrationReasons.Any())
            {
                normalizedColumns = incomingColumns
                    .Select(c => new Column
                    {
                        StorageKey = c.StorageKey,
                        Name = c.Name,
                        Type = c.Type,
                        Required = c.Required,
                        AllowedValues = c.AllowedValues,
                        MinNumber = c.MinNumber,
                        MaxNumber = c.MaxNumber,
                        Regex = c.Regex
                    })
                    .ToList();

                foreach (var column in normalizedColumns.Where(c => string.IsNullOrWhiteSpace(c.StorageKey)))
                {
                    if (currentByName.TryGetValue(column.Name, out var existing) &&
                        !string.IsNullOrWhiteSpace(existing.StorageKey))
                    {
                        column.StorageKey = existing.StorageKey;
                    }
                }

                normalizedColumns = AssignStorageKeys(normalizedColumns);
            }
        }

        Status[]? statusesToPersist = null;
        if (statuses is not null)
        {
            // Disallow deletions or renames (by name)
            var currentNames =
                new HashSet<string>(currentStatuses.Select(s => s.Name), StringComparer.OrdinalIgnoreCase);
            var nextNames = new HashSet<string>(statuses.Select(s => s.Name), StringComparer.OrdinalIgnoreCase);
            var deletedStatuses = currentNames.Where(n => !nextNames.Contains(n)).ToArray();
            if (deletedStatuses.Length > 0)
            {
                migrationReasons.Add(
                    $"Removing statuses [{string.Join(", ", deletedStatuses)}] requires a migration.");

                foreach (var statusName in deletedStatuses)
                {
                    if (!removeStatuses.ContainsKey(statusName))
                    {
                        removeStatuses[statusName] = new RemoveStatusOp(statusName, null);
                    }
                }
            }
            else
            {
                statusesToPersist = statuses.ToArray();
            }
        }

        if (migrationReasons.Count > 0)
        {
            var nextColumnsForPlan = normalizedColumns ?? requestedColumns ?? currentColumns.ToList();
            var nextStatusesForPlan = statusesToPersist ?? currentStatuses;

            var plan = MigrationPlanBuilder.Build(
                currentColumns,
                nextColumnsForPlan,
                nextStatusesForPlan,
                migrationReasons);

            throw new ListMigrationRequiredException(migrationReasons, plan);
        }

        if (normalizedColumns is not null)
        {
            await unitOfWork.ListsStore.SetColumnsAsync(list, normalizedColumns, updatedBy, cancellationToken);
        }

        if (statusesToPersist is not null)
        {
            // Color changes and additions are allowed
            await unitOfWork.ListsStore.SetStatusesAsync(list, statusesToPersist, updatedBy, cancellationToken);
        }

        if (transitions is not null)
        {
            await unitOfWork.ListsStore.SetStatusTransitionsAsync(list, transitions, updatedBy, cancellationToken);
        }

        // Emit a domain event to capture update action
        events.Enqueue(new ListUpdatedEvent(list, updatedBy), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<TItem?> GetItemByIdAsync(
        TList list,
        int itemId,
        CancellationToken cancellationToken = default
    )
    {
        if (list.Id is null)
        {
            throw new InvalidOperationException("List must have an Id");
        }

        var retval = await unitOfWork.ItemsStore.GetByIdAsync(itemId, list.Id.Value, cancellationToken);
        return retval;
    }

    public async Task<TItem> CreateItemAsync(
        TList list,
        object bag,
        string createdBy,
        CancellationToken cancellationToken = default
    )
    {
        if (list.Id is null)
        {
            throw new InvalidOperationException("List must have an Id");
        }

        await bagValidator.ValidateAsync(list, bag, cancellationToken);

        var retval = await unitOfWork.ItemsStore.InitAsync(list.Id.Value, createdBy, cancellationToken);
        await unitOfWork.ItemsStore.SetBagAsync(retval, bag, createdBy, cancellationToken);
        await unitOfWork.ItemsStore.CreateAsync(retval, cancellationToken);
        events.Enqueue(new ListItemCreatedEvent(retval, createdBy), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return retval;
    }

    public async Task<IEnumerable<TItem>> CreateItemsAsync(
        TList list,
        IEnumerable<object> bags,
        string createdBy,
        CancellationToken cancellationToken = default
    )
    {
        if (list.Id is null)
        {
            throw new InvalidOperationException("List must have an Id");
        }

        var retval = new List<TItem>();
        foreach (var bag in bags)
        {
            await bagValidator.ValidateAsync(list, bag, cancellationToken);
            var item = await unitOfWork.ItemsStore.InitAsync(list.Id.Value, createdBy, cancellationToken);
            await unitOfWork.ItemsStore.SetBagAsync(item, bag, createdBy, cancellationToken);
            await unitOfWork.ItemsStore.CreateAsync(item, cancellationToken);
            retval.Add(item);
        }

        foreach (var item in retval)
        {
            events.Enqueue(new ListItemCreatedEvent(item, createdBy), EventPhase.AfterSave);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return retval;
    }

    public async Task UpdateItemAsync(
        TList list,
        TItem item,
        object newBag,
        string updatedBy,
        CancellationToken cancellationToken = default
    )
    {
        if (list.Id is null)
        {
            throw new InvalidOperationException("List must have an Id");
        }

        await bagValidator.ValidateAsync(list, newBag, cancellationToken);

        // Transition validation (if status changes)
        var oldBagObj = await unitOfWork.ItemsStore.GetBagAsync(item, cancellationToken);
        var newDict = newBag as IDictionary<string, object?>;
        var oldStatus = oldBagObj is IDictionary<string, object?> oldDict && oldDict.TryGetValue("status", out var os)
            ? os as string
            : null;
        var newStatus = newDict != null && newDict.TryGetValue("status", out var ns) ? ns as string : null;

        if (!string.IsNullOrWhiteSpace(oldStatus) && !string.IsNullOrWhiteSpace(newStatus) &&
            !string.Equals(oldStatus, newStatus, StringComparison.OrdinalIgnoreCase))
        {
            var transitions = await unitOfWork.ListsStore.GetStatusTransitionsAsync(list, cancellationToken);
            if (transitions.Length > 0)
            {
                var allowed = transitions
                    .FirstOrDefault(t => string.Equals(t.From, oldStatus, StringComparison.OrdinalIgnoreCase))
                    ?.AllowedNext
                    .Any(s => string.Equals(s, newStatus, StringComparison.OrdinalIgnoreCase)) ?? false;
                if (!allowed)
                {
                    throw new InvalidOperationException(
                        $"Transition from '{oldStatus}' to '{newStatus}' is not allowed");
                }
            }
        }

        await unitOfWork.ItemsStore.SetBagAsync(item, newBag, updatedBy, cancellationToken);
        events.Enqueue(new ListItemUpdatedEvent(item, updatedBy, oldBagObj, newBag), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteItemAsync(
        TItem item,
        string deletedBy,
        CancellationToken cancellationToken = default
    )
    {
        await unitOfWork.ItemsStore.DeleteAsync(item, deletedBy, cancellationToken);
        events.Enqueue(new ListItemDeletedEvent(item, deletedBy), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public Task<object> GetItemBagAsync(TItem item, CancellationToken cancellationToken = default)
    {
        return unitOfWork.ItemsStore.GetBagAsync(item, cancellationToken);
    }

    public Task<Column[]> GetColumnsAsync(TList list, CancellationToken cancellationToken = default)
    {
        return unitOfWork.ListsStore.GetColumnsAsync(list, cancellationToken);
    }

    public Task<Status[]> GetStatusesAsync(TList list, CancellationToken cancellationToken = default)
    {
        return unitOfWork.ListsStore.GetStatusesAsync(list, cancellationToken);
    }

    public Task<StatusTransition[]> GetStatusTransitionsAsync(
        TList list,
        CancellationToken cancellationToken = default
    )
    {
        return unitOfWork.ListsStore.GetStatusTransitionsAsync(list, cancellationToken);
    }

    public async Task<object> CreateExampleBagAsync(TList list, CancellationToken cancellationToken = default)
    {
        dynamic retval = new ExpandoObject();
        var typeMap = new Dictionary<ColumnType, object>
        {
            { ColumnType.Text, "text" },
            { ColumnType.Number, 77 },
            { ColumnType.Date, DateTime.Today.ToString("O") },
            { ColumnType.Boolean, true }
        };

        var columns = await unitOfWork.ListsStore.GetColumnsAsync(list, cancellationToken);
        foreach (var column in columns)
        {
            var cast = (IDictionary<string, object?>)retval;
            cast[column.Property] = typeMap[column.Type];
        }

        var statuses = await unitOfWork.ListsStore.GetStatusesAsync(list, cancellationToken);
        retval.status = statuses.FirstOrDefault()?.Name;
        return retval;
    }

    private sealed class TightenConstraintsBuilder
    {
        public TightenConstraintsBuilder(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public bool? Required { get; set; }

        public string[]? AllowedValues { get; set; }

        public decimal? MinNumber { get; set; }

        public decimal? MaxNumber { get; set; }

        public string? Regex { get; set; }

        public TightenConstraintsOp ToOp()
        {
            return new TightenConstraintsOp(Key, Required, AllowedValues, MinNumber, MaxNumber, Regex);
        }
    }
}