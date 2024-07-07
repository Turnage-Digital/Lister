using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

public class ListsStore(ListerDbContext dbContext)
    : IListsStore<ListEntity, ItemEntity>
{
    private readonly EntityStore<ListEntity> _entityStore = new(dbContext);

    public Task<ListEntity> InitAsync(string createdBy, string name, CancellationToken cancellationToken = default)
    {
        var retval = new ListEntity
        {
            Name = name,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };
        return Task.FromResult(retval);
    }

    public async Task CreateAsync(ListEntity list, CancellationToken cancellationToken = default)
    {
        await _entityStore.CreateAsync(list, cancellationToken);
    }

    public async Task<ListEntity?> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        var parsed = Guid.Parse(id);
        var retval = await _entityStore.ReadAsync(parsed, cancellationToken);
        return retval;
    }

    public Task DeleteAsync(ListEntity list, string deletedBy, CancellationToken cancellationToken = default)
    {
        list.DeletedBy = deletedBy;
        list.DeletedOn = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public async Task<ListEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var retval = await dbContext.Lists
            .SingleOrDefaultAsync(l => l.Name == name, cancellationToken);
        return retval;
    }

    public Task SetColumnsAsync(
        ListEntity list,
        IEnumerable<Column> columns,
        CancellationToken cancellationToken = default
    )
    {
        list.Columns = columns
            .Select(pd => new ColumnEntity { Name = pd.Name, Type = pd.Type, List = list })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<Column[]> GetColumnsAsync(ListEntity list, CancellationToken cancellationToken = default)
    {
        var retval = list.Columns
            .Select(pd => new Column { Name = pd.Name, Type = pd.Type })
            .ToArray();
        return Task.FromResult(retval);
    }

    public Task SetStatusesAsync(
        ListEntity list,
        IEnumerable<Status> statuses,
        CancellationToken cancellationToken = default
    )
    {
        list.Statuses = statuses
            .Select(sd => new StatusEntity { Name = sd.Name, Color = sd.Color, List = list })
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

    public Task AddItemAsync(ListEntity list, ItemEntity item, CancellationToken cancellationToken = default)
    {
        list.Items.Add(item);
        return Task.CompletedTask;
    }
}