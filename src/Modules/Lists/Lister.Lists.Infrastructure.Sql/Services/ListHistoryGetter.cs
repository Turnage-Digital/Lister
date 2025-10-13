using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListHistoryGetter(ListsDbContext dbContext) : IGetListHistory
{
    public async Task<HistoryPage<ListHistoryType>> GetAsync(
        Guid listId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext.Lists
            .Where(list => list.Id == listId)
            .SelectMany(list => list.History)
            .OrderByDescending(entry => entry.On);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(entry => new Entry<ListHistoryType>
            {
                By = entry.By,
                On = entry.On,
                Type = entry.Type,
                Bag = entry.Bag
            })
            .ToArrayAsync(cancellationToken);

        return new HistoryPage<ListHistoryType>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}