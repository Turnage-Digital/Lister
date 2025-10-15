using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetItemDetails;

public class GetItemDetailsQueryHandler(IGetItemDetails itemDetailsGetter)
    : IRequestHandler<GetItemDetailsQuery, ItemDetails?>
{
    public async Task<ItemDetails?> Handle(GetItemDetailsQuery request, CancellationToken cancellationToken)
    {
        var retval = await itemDetailsGetter.GetAsync(request.ListId, request.ItemId, cancellationToken);
        return retval;
    }
}