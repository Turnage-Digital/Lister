using Lister.Core.Sql.Entities;

namespace Lister.Core.Sql;

public class ItemsStore : IItemsStore<ItemDb>
{
    public Task<ItemDb> InitAsync(string createdBy, object bag, CancellationToken cancellationToken)
    {
        var itemEntity = new ItemDb
        {
            Bag = bag,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };
        return Task.FromResult(itemEntity);
    }
}