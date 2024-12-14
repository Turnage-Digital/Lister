using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql;

public class ItemsStore(ListsDbContext dbContext)
    : IItemsStore<ItemDb>
{
    private readonly EntityStore<ItemDb> _entityStore = new(dbContext);

    public async Task<ItemDb?> GetByIdAsync(int itemId, Guid listId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Items
            .Where(i => i.Id == itemId)
            .Where(i => i.ListId == listId)
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }

    public Task<ItemDb> InitAsync(Guid listId, string createdBy, CancellationToken cancellationToken)
    {
        var retval = new ItemDb
        {
            ListId = listId
        };
        retval.History.Add(new ItemHistoryEntryDb
        {
            Type = ItemHistoryType.Created,
            On = DateTime.UtcNow,
            By = createdBy,
            Item = retval
        });
        return Task.FromResult(retval);
    }

    public async Task CreateAsync(ItemDb item, CancellationToken cancellationToken)
    {
        await _entityStore.CreateAsync(item, cancellationToken);
    }

    public Task DeleteAsync(ItemDb item, string deletedBy, CancellationToken cancellationToken)
    {
        item.IsDeleted = true;
        item.History.Add(new ItemHistoryEntryDb
        {
            Type = ItemHistoryType.Deleted,
            On = DateTime.UtcNow,
            By = deletedBy,
            Item = item
        });
        return Task.CompletedTask;
    }

    public Task SetBagAsync(ItemDb item, object bag, CancellationToken cancellationToken)
    {
        item.Bag = bag;
        return Task.CompletedTask;
    }

    public Task<object> GetBagAsync(ItemDb item, CancellationToken cancellationToken)
    {
        return Task.FromResult(item.Bag);
    }
}