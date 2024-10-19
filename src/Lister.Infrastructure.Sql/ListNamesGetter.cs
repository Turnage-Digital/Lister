using Lister.Domain;
using Lister.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Infrastructure.Sql;

public class ListNamesGetter(ListerDbContext dbContext) : IGetListNames
{
    public async Task<ListName[]> Get(string userId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Select(list => new ListName
            {
                Id = list.Id,
                Name = list.Name
            })
            .ToArrayAsync(cancellationToken);
        return retval;
    }
}