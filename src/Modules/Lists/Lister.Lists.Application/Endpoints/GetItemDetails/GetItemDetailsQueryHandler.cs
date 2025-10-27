using Lister.Lists.ReadOnly.Queries;
using Lister.Lists.ReadOnly.Dtos;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetItemDetails;

public class GetItemDetailsQueryHandler(IGetItemDetails itemDetailsGetter)
    : IRequestHandler<GetItemDetailsQuery, ItemDetailsDto?>
{
    public async Task<ItemDetailsDto?> Handle(GetItemDetailsQuery request, CancellationToken cancellationToken)
    {
        var retval = await itemDetailsGetter.GetAsync(request.ListId, request.ItemId, cancellationToken);
        return retval;
    }
}
