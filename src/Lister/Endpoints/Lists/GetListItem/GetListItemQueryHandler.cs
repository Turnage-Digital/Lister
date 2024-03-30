using Lister.Core.SqlDB;
using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.Endpoints.Lists.GetListItem;

public class GetListItemQueryHandler : IRequestHandler<GetListItemQuery, Item>
{
    private readonly ListerDbContext _dbContext;

    public GetListItemQueryHandler(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Item> Handle(GetListItemQuery request, CancellationToken cancellationToken)
    {
        var parsedListId = Guid.Parse(request.ListId);
        var parsedItemId = int.Parse(request.ItemId);
        var retval = await _dbContext.Items
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