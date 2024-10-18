using Lister.Domain;
using Lister.Domain.Entities;
using MediatR;

namespace Lister.Application.Queries;

public class GetListItemsQueryHandler(IGetListItems listItemsGetter)
    : IRequestHandler<GetListItemsQuery, PagedResponse<Item>>
{
    public async Task<PagedResponse<Item>> Handle(
        GetListItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var parsedListId = Guid.Parse(request.ListId);
        var retval = await listItemsGetter.Get(request.UserId,
            parsedListId, request.Page, request.PageSize, request.Field, request.Sort, cancellationToken);
        return retval;
    }
}