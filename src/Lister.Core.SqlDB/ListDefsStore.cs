using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB;

public class ListDefsStore : IListDefsStore<ListDefEntity>
{
    private readonly ListerDbContext _dbContext;
    private readonly EntityStore<ListDefEntity> _entityStore;

    public ListDefsStore(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
        _entityStore = new EntityStore<ListDefEntity>(dbContext);
    }

    public Task<ListDefEntity> InitAsync(CancellationToken cancellationToken = default)
    {
        var retval = new ListDefEntity();
        return Task.FromResult(retval);
    }

    public async Task CreateAsync(ListDefEntity listDef, CancellationToken cancellationToken = default)
    {
        listDef.CreatedOn = DateTime.UtcNow;
        await _entityStore.CreateAsync(listDef, cancellationToken);
    }

    public async Task<ListDefEntity?> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        var parsed = Guid.Parse(id);
        var retval = await _entityStore.ReadAsync(parsed, cancellationToken);
        return retval;
    }

    public Task SetNameAsync(ListDefEntity listDef, string name, CancellationToken cancellationToken = default)
    {
        listDef.Name = name;
        return Task.CompletedTask;
    }

    public Task<string> GetNameAsync(ListDefEntity listDef, CancellationToken cancellationToken = default)
    {
        var retval = listDef.Name;
        return Task.FromResult(retval);
    }

    public Task SetStatusDefsAsync(
        ListDefEntity listDef,
        StatusDef[] statusDefs,
        CancellationToken cancellationToken = default
    )
    {
        listDef.StatusDefs = statusDefs
            .Select(sd => new StatusDefEntity { Name = sd.Name, Color = sd.Color })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<StatusDef[]> GetStatusDefsAsync(ListDefEntity listDef, CancellationToken cancellationToken = default)
    {
        var retval = listDef.StatusDefs
            .Select(sd => new StatusDef { Name = sd.Name, Color = sd.Color })
            .ToArray();
        return Task.FromResult(retval);
    }

    public Task SetColumnDefsAsync(
        ListDefEntity listDef,
        ColumnDef[] columnDefs,
        CancellationToken cancellationToken = default
    )
    {
        listDef.ColumnDefs = columnDefs
            .Select(pd => new ColumnDefEntity { Name = pd.Name, Type = pd.Type })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<ColumnDef[]> GetColumnDefsAsync(ListDefEntity listDef, CancellationToken cancellationToken)
    {
        var retval = listDef.ColumnDefs
            .Select(pd => new ColumnDef { Name = pd.Name, Type = pd.Type })
            .ToArray();
        return Task.FromResult(retval);
    }

    public Task SetCreatedByAsync(ListDefEntity listDef, string userId, CancellationToken cancellationToken)
    {
        listDef.CreatedBy = userId;
        return Task.CompletedTask;
    }

    public Task<string> GetCreatedByAsync(ListDefEntity listDef, CancellationToken cancellationToken)
    {
        var retval = listDef.CreatedBy;
        return Task.FromResult(retval);
    }

    public Task<DateTime> GetCreatedOnAsync(ListDefEntity listDef, CancellationToken cancellationToken)
    {
        var retval = listDef.CreatedOn;
        return Task.FromResult(retval);
    }
}