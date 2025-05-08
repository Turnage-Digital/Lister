using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetItemDetails;

public class GetItemDetailsQueryHandler(IGetItemDetails itemDetailsGetter)
    : IRequestHandler<GetItemDetailsQuery, ItemDetails?>
{
    public async Task<ItemDetails?> Handle(GetItemDetailsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var parsedListId = Guid.Parse(request.ListId);
        var retval = await itemDetailsGetter.GetAsync(parsedListId, request.ItemId, cancellationToken);
        return retval;
    }
}