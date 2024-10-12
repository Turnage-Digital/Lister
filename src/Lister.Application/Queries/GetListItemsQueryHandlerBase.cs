using Lister.Core.Entities;
using MediatR;

namespace Lister.Application.Queries;

public abstract class GetListItemsQueryHandlerBase
    : IRequestHandler<GetListItemsQuery, PagedResponse<Item>>
{
    public abstract Task<PagedResponse<Item>> Handle(
        GetListItemsQuery request,
        CancellationToken cancellationToken);
}