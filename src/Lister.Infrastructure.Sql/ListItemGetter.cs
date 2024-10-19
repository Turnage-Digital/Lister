using Lister.Domain;
using Lister.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lister.Infrastructure.Sql;

public class ListItemGetter(ListerDbContext dbContext) : IGetListItem
{
    public async Task<Item?> Get(string userId, Guid listId, int itemId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Items
            .Where(item => item.ListDb.CreatedBy == userId)
            .Where(item => item.ListId == listId)
            .Where(item => item.Id == itemId)
            .Select(item => new Item
            {
                Id = item.Id,
                Bag = item.Bag
            })
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}