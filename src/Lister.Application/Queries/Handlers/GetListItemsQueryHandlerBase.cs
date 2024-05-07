using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Application.Queries.Handlers;

public abstract class GetListItemsQueryHandlerBase
    : IRequestHandler<GetListItemsQuery, PagedResponse<Item>>
{
    public abstract Task<PagedResponse<Item>> Handle(
        GetListItemsQuery request,
        CancellationToken cancellationToken);
}