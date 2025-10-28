using Lister.Lists.ReadOnly.Dtos;
using Lister.Lists.ReadOnly.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListNamesGetter(ListsDbContext dbContext) : IGetListNames
{
    public async Task<ListNameDto[]> GetAsync(CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Where(list => list.IsDeleted == false)
            .Select(list => new ListNameDto
            {
                Id = list.Id,
                Name = list.Name,
                Count = list.Items.Count(item => item.IsDeleted == false)
            })
            .ToArrayAsync(cancellationToken);
        return retval;
    }
}