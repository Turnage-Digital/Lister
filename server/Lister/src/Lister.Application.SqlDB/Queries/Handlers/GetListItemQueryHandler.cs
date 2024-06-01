using Lister.Application.Queries;
using Lister.Application.Queries.Handlers;
using Lister.Core.SqlDB;
using Lister.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Application.SqlDB.Queries.Handlers;

public class GetListItemQueryHandler(ListerDbContext dbContext)
    : GetListItemQueryHandlerBase
{
    public override async Task<Item> Handle(GetListItemQuery request, CancellationToken cancellationToken)
    {
        var parsedListId = Guid.Parse(request.ListId);
        var parsedItemId = int.Parse(request.ItemId);
        var retval = await dbContext.Items
            .Where(item => item.List.CreatedBy == request.UserId)
            .Where(item => item.ListId == parsedListId)
            .Where(item => item.Id == parsedItemId)
            .Select(item => new Item
            {
                Id = item.Id,
                Bag = item.Bag
            })
            .SingleAsync(cancellationToken);
        return retval;
    }
}