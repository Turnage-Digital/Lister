using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Queries.GetPagedList;

public class GetPagedListQueryHandler(IGetPagedList pagedListGetter)
    : IRequestHandler<GetPagedListQuery, PagedList>
{
    public async Task<PagedList> Handle(
        GetPagedListQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "UserId is null");
        }

        var parsedListId = Guid.Parse(request.ListId);
        var retval = await pagedListGetter.GetAsync(
            parsedListId,
            request.Page,
            request.PageSize,
            request.Field,
            request.Sort,
            cancellationToken);
        return retval;
    }
}