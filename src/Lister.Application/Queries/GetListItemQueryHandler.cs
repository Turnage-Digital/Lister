using Lister.Domain;
using Lister.Domain.Entities;
using MediatR;

namespace Lister.Application.Queries;

public class GetListItemQueryHandler(IGetListItem listItemGetter)
    : IRequestHandler<GetListItemQuery, Item?>
{
    public async Task<Item?> Handle(GetListItemQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var parsedListId = Guid.Parse(request.ListId);
        var retval = await listItemGetter.Get(request.UserId, parsedListId, request.ItemId, cancellationToken);
        return retval;
    }
}