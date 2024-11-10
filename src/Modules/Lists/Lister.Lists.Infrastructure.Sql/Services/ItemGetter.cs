using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ItemGetter(ListerDbContext dbContext) : IGetItem
{
    public async Task<ItemDetails?> GetAsync(Guid listId, int itemId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Items
            .Where(item => item.ListId == listId)
            .Where(item => item.Id == itemId)
            .Where(item => item.IsDeleted == false)
            .Select(item => new ItemDetails
            {
                Bag = item.Bag,
                ListId = item.ListId,
                Id = item.Id
            })
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}