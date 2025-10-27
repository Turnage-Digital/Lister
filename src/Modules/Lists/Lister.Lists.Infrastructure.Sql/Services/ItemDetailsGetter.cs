using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using Lister.Lists.ReadOnly.Queries;
using Lister.Lists.ReadOnly.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ItemDetailsGetter(ListsDbContext dbContext) : IGetItemDetails
{
    public async Task<ItemDetailsDto?> GetAsync(Guid listId, int itemId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Items
            .Where(item => item.ListId == listId)
            .Where(item => item.Id == itemId)
            .Where(item => item.IsDeleted == false)
            .Select(item => new ItemDetailsDto
            {
                Bag = item.Bag,
                ListId = item.ListId,
                Id = item.Id,
                History = item.History
                    .OrderByDescending(entry => entry.On)
                    .Select(entry => new Entry<ItemHistoryType>
                    {
                        By = entry.By,
                        On = entry.On,
                        Type = entry.Type,
                        Bag = entry.Bag
                    })
                    .ToArray()
            })
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}
