using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetPagedList;

public class GetPagedListQueryHandler(IGetPagedList pagedListGetter)
    : IRequestHandler<GetPagedListQuery, PagedList>
{
    public async Task<PagedList> Handle(
        GetPagedListQuery request,
        CancellationToken cancellationToken
    )
    {
        var retval = await pagedListGetter.GetAsync(
            request.ListId,
            request.Page,
            request.PageSize,
            request.Field,
            request.Sort,
            cancellationToken);
        return retval;
    }
}