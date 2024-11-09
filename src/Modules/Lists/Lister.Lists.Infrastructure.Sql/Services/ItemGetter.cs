using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ItemGetter(ListerDbContext dbContext) : IGetItem
{
    public async Task<Item?> GetAsync(Guid listId, int itemId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Items
            .Where(item => item.ListId == listId)
            .Where(item => item.Id == itemId)
            .Where(item => item.IsDeleted == false)
            .Select(item => new Item
            {
                Bag = item.Bag,
                ListId = item.ListId,
                Id = item.Id
            })
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}