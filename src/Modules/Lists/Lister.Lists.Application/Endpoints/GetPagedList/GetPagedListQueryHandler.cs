using Lister.Lists.ReadOnly.Queries;
using Lister.Lists.ReadOnly.Dtos;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetPagedList;

public class GetPagedListQueryHandler(IGetPagedList pagedListGetter)
    : IRequestHandler<GetPagedListQuery, PagedListDto>
{
    public async Task<PagedListDto> Handle(
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
