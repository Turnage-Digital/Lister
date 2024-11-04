using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListItemGetter(ListerDbContext dbContext) : IGetListItem
{
    public async Task<Item?> GetAsync(string userId, Guid listId, int itemId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Items
            .Where(item => item.ListDb.CreatedBy == userId)
            .Where(item => item.ListId == listId)
            .Where(item => item.Id == itemId)
            // .Select(item => new Item
            // {
            //     Id = item.Id,
            //     Bag = item.Bag
            // })
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}