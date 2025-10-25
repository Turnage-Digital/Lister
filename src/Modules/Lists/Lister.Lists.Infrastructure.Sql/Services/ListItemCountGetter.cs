using Lister.Lists.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListItemCountGetter(ListsDbContext dbContext) : IGetListItemCount
{
    public Task<int> CountAsync(Guid listId, CancellationToken cancellationToken)
    {
        var retval = dbContext.Items
            .Where(i => i.ListId == listId)
            .CountAsync(cancellationToken);
        return retval;
    }
}