using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListNamesGetter(ListsDbContext dbContext) : IGetListNames
{
    public async Task<ListName[]> GetAsync(CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Where(list => list.IsDeleted == false)
            .Select(list => new ListName
            {
                Id = list.Id,
                Name = list.Name,
                Count = list.Items.Count(item => item.IsDeleted == false)
            })
            .ToArrayAsync(cancellationToken);
        return retval;
    }
}