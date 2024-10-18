using Lister.Domain;
using Lister.Infra.Sql.Entities;

namespace Lister.Infra.Sql;

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