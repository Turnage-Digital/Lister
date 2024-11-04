using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql;

public class ListsStore(ListerDbContext dbContext)
    : IListsStore<ListDb>
{
    private readonly EntityStore<ListDb> _entityStore = new(dbContext);

    public async Task<ListDb?> GetByIdAsync(string userId, Guid id, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }

    public async Task<ListDb?> GetByNameAsync(string userId, string name, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Name == name)
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }

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

    public Task DeleteAsync(ListDb listDB, string deletedBy, CancellationToken cancellationToken = default)
    {
        listDB.DeletedBy = deletedBy;
        listDB.DeletedOn = DateTime.UtcNow;
        return Task.CompletedTask;
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

    public Task<Item> AddItemAsync(
        ListDb listDB,
        string createdBy,
        object bag,
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

    public async Task DeleteItemAsync(ListDb list, string deletedBy, int itemId, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items
            .Where(i => i.ListDb == list)
            .Where(i => i.Id == itemId)
            .SingleOrDefaultAsync(cancellationToken);

        if (item is null)
            throw new InvalidOperationException($"Item with id {itemId} does not exist");

        item.DeletedBy = deletedBy;
        item.DeletedOn = DateTime.UtcNow;
    }
}