using Lister.Core.Domain;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetListItems;

public class GetListItemsQueryHandler(IGetListItems listItemsGetter)
    : IRequestHandler<GetListItemsQuery, PagedResponse<ListItem>>
{
    public async Task<PagedResponse<ListItem>> Handle(
        GetListItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var parsedListId = Guid.Parse(request.ListId);
        var retval = await listItemsGetter.GetAsync(
            parsedListId,
            request.Page,
            request.PageSize,
            request.Field,
            request.Sort,
            cancellationToken);
        return retval;
    }
}