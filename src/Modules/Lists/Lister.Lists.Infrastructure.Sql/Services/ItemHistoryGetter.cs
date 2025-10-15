using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ItemHistoryGetter(ListsDbContext dbContext) : IGetItemHistory
{
    public async Task<HistoryPage<ItemHistoryType>> GetAsync(
        Guid listId,
        int itemId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext.Items
            .Where(item => item.ListId == listId)
            .Where(item => item.Id == itemId)
            .SelectMany(item => item.History)
            .OrderByDescending(entry => entry.On);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(entry => new Entry<ItemHistoryType>
            {
                By = entry.By,
                On = entry.On,
                Type = entry.Type,
                Bag = entry.Bag
            })
            .ToArrayAsync(cancellationToken);

        return new HistoryPage<ItemHistoryType>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}