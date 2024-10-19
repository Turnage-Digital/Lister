using Lister.Domain;
using Lister.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lister.Infrastructure.Sql;

public class ListGetter(ListerDbContext dbContext) : IGetList<ListDb>
{
    public async Task<ListDb?> Get(string userId, Guid listId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Id == listId)
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .AsSplitQuery()
            .SingleAsync(cancellationToken);
        return retval;
    }
}