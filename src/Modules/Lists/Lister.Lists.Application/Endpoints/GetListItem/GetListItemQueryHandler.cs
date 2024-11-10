using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetListItem;

public class GetListItemQueryHandler(IGetItem itemGetter)
    : IRequestHandler<GetListItemQuery, ItemDetails?>
{
    public async Task<ItemDetails?> Handle(GetListItemQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var parsedListId = Guid.Parse(request.ListId);
        var retval = await itemGetter.GetAsync(parsedListId, request.ItemId, cancellationToken);
        return retval;
    }
}