using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql;

public class ListsStore(ListerDbContext dbContext)
    : IListsStore<ListDb>
{
    private readonly EntityStore<ListDb> _entityStore = new(dbContext);

    public async Task<ListDb?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .Where(list => list.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }

    public async Task<ListDb?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .Where(list => list.Name == name)
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }

    public Task<ListDb> InitAsync(string name, string createdBy, CancellationToken cancellationToken)
    {
        var retval = new ListDb
        {
            Name = name
        };
        return Task.FromResult(retval);
    }

    public async Task CreateAsync(ListDb listDB, CancellationToken cancellationToken = default)
    {
        await _entityStore.CreateAsync(listDB, cancellationToken);
    }

    public Task DeleteAsync(ListDb listDB, string deletedBy, CancellationToken cancellationToken = default)
    {
        listDB.IsDeleted = true;

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
    
    public async Task<Item?> GetItemByIdAsync(int id, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Items
            .Where(i => i.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
    
    public Task<Item> CreateItemAsync(ListDb listDB,
        object bag,
        string createdBy,
        CancellationToken cancellationToken)
    {
        var itemDB = new ItemDb
        {
            Bag = bag,
            List = listDB
        };
        itemDB.History.Add(new ItemHistoryEntryDb
        {
            Type = ItemHistoryType.Created,
            On = DateTime.UtcNow,
            By = createdBy,
            Item = itemDB
        });
        listDB.Items.Add(itemDB);
        return Task.FromResult<Item>(itemDB);
    }

    public Task DeleteItemAsync(ListDb list, Item item, string deletedBy, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteItemAsync(ListDb list, ItemDb item, string deletedBy, int itemId,
        CancellationToken cancellationToken)
    {
        // var item = await dbContext.Items
        //     .Where(i => i.ListDb == list)
        //     .Where(i => i.Id == itemId)
        //     .SingleOrDefaultAsync(cancellationToken);

        if (item is null)
            throw new InvalidOperationException($"Item with id {itemId} does not exist");
        return Task.CompletedTask;
    }
}