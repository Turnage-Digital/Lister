using Lister.Application.Queries;
using Lister.Core.Entities;
using Lister.Core.Sql;
using Microsoft.EntityFrameworkCore;

namespace Lister.Application.Sql.Queries;

public class GetListItemQueryHandler(ListerDbContext dbContext)
    : GetListItemQueryHandlerBase
{
    public override async Task<Item?> Handle(GetListItemQuery request, CancellationToken cancellationToken)
    {
        var parsedListId = Guid.Parse(request.ListId);
        var retval = await dbContext.Items
            .Where(item => item.ListDb.CreatedBy == request.UserId)
            .Where(item => item.ListId == parsedListId)
            .Where(item => item.Id == request.ItemId)
            .Select(item => new Item
            {
                Id = item.Id,
                Bag = item.Bag
            })
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}