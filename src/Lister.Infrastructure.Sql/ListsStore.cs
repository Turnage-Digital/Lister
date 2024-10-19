using Lister.Domain;
using Lister.Domain.Entities;
using Lister.Domain.ValueObjects;
using Lister.Infrastructure.Sql.Entities;
using Lister.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Infrastructure.Sql;

public class ListsStore(ListerDbContext dbContext)
    : IListsStore<ListDb>
{
    private readonly EntityStore<ListDb> _entityStore = new(dbContext);

    public Task<ListDb> InitAsync(string createdBy, string name, CancellationToken cancellationToken = default)
    {
        var retval = new ListDb
        {
            Name = name,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };
        return Task.FromResult(retval);
    }

    public async Task CreateAsync(ListDb listDB, CancellationToken cancellationToken = default)
    {
        await _entityStore.CreateAsync(listDB, cancellationToken);
    }

    public async Task<ListDb?> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        var parsed = Guid.Parse(id);
        var retval = await _entityStore.ReadAsync(parsed, cancellationToken);
        return retval;
    }

    public Task DeleteAsync(ListDb listDB, string deletedBy, CancellationToken cancellationToken = default)
    {
        listDB.DeletedBy = deletedBy;
        listDB.DeletedOn = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public async Task<ListDb?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var retval = await dbContext.Lists
            .SingleOrDefaultAsync(l => l.Name == name, cancellationToken);
        return retval;
    }

    public Task SetColumnsAsync(
        ListDb listDB,
        IEnumerable<Column> columns,
        CancellationToken cancellationToken = default
    )
    {
        listDB.Columns = columns
            .Select(pd => new ColumnDb { Name = pd.Name, Type = pd.Type, ListDb = listDB })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<Column[]> GetColumnsAsync(ListDb listDB, CancellationToken cancellationToken = default)
    {
        var retval = listDB.Columns
            .Select(pd => new Column { Name = pd.Name, Type = pd.Type })
            .ToArray();
        return Task.FromResult(retval);
    }

    public Task SetStatusesAsync(
        ListDb listDB,
        IEnumerable<Status> statuses,
        CancellationToken cancellationToken = default
    )
    {
        listDB.Statuses = statuses
            .Select(sd => new StatusDb { Name = sd.Name, Color = sd.Color, ListDb = listDB })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<Status[]> GetStatusesAsync(ListDb listDB, CancellationToken cancellationToken = default)
    {
        var retval = listDB.Statuses
            .Select(sd => new Status { Name = sd.Name, Color = sd.Color })
            .ToArray();
        return Task.FromResult(retval);
    }

    public Task<Item> AddItemAsync(ListDb listDB, string createdBy, object bag,
        CancellationToken cancellationToken = default)
    {
        var itemDB = new ItemDb
        {
            Bag = bag,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow,
            ListDb = listDB
        };
        listDB.Items.Add(itemDB);
        return Task.FromResult<Item>(itemDB);
    }
}