using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB;

public class ListsStore : IListsStore<ListEntity>
{
    private readonly ListerDbContext _dbContext;
    private readonly EntityStore<ListEntity> _entityStore;

    public ListsStore(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
        _entityStore = new EntityStore<ListEntity>(dbContext);
    }

    public Task<ListEntity> InitAsync(CancellationToken cancellationToken = default)
    {
        var retval = new ListEntity();
        return Task.FromResult(retval);
    }

    public async Task CreateAsync(ListEntity list, CancellationToken cancellationToken = default)
    {
        list.CreatedOn = DateTime.UtcNow;
        await _entityStore.CreateAsync(list, cancellationToken);
    }

    public async Task<ListEntity?> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        var parsed = Guid.Parse(id);
        var retval = await _entityStore.ReadAsync(parsed, cancellationToken);
        return retval;
    }

    public Task SetNameAsync(ListEntity list, string name, CancellationToken cancellationToken = default)
    {
        list.Name = name;
        return Task.CompletedTask;
    }

    public Task<string> GetNameAsync(ListEntity list, CancellationToken cancellationToken = default)
    {
        var retval = list.Name;
        return Task.FromResult(retval);
    }

    public Task SetStatusesAsync(
        ListEntity list,
        Status[] statuses,
        CancellationToken cancellationToken = default
    )
    {
        list.Statuses = statuses
            .Select(sd => new StatusEntity { Name = sd.Name, Color = sd.Color })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<Status[]> GetStatusesAsync(ListEntity list, CancellationToken cancellationToken = default)
    {
        var retval = list.Statuses
            .Select(sd => new Status { Name = sd.Name, Color = sd.Color })
            .ToArray();
        return Task.FromResult(retval);
    }

    public Task SetColumnsAsync(
        ListEntity list,
        Column[] columns,
        CancellationToken cancellationToken = default
    )
    {
        list.Columns = columns
            .Select(pd => new ColumnEntity { Name = pd.Name, Type = pd.Type })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<Column[]> GetColumnsAsync(ListEntity list, CancellationToken cancellationToken)
    {
        var retval = list.Columns
            .Select(pd => new Column { Name = pd.Name, Type = pd.Type })
            .ToArray();
        return Task.FromResult(retval);
    }

    public Task SetCreatedByAsync(ListEntity list, string userId, CancellationToken cancellationToken)
    {
        list.CreatedBy = userId;
        return Task.CompletedTask;
    }

    public Task<string> GetCreatedByAsync(ListEntity list, CancellationToken cancellationToken)
    {
        var retval = list.CreatedBy;
        return Task.FromResult(retval);
    }

    public Task<DateTime> GetCreatedOnAsync(ListEntity list, CancellationToken cancellationToken)
    {
        var retval = list.CreatedOn;
        return Task.FromResult(retval);
    }
}