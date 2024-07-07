using Lister.Core.SqlDB.Entities;

namespace Lister.Core.SqlDB;

public class ItemsStore : IItemsStore<ItemEntity>
{
    public Task<ItemEntity> InitAsync(string createdBy, object bag, CancellationToken cancellationToken)
    {
        var itemEntity = new ItemEntity
        {
            Bag = bag,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };
        return Task.FromResult(itemEntity);
    }
}