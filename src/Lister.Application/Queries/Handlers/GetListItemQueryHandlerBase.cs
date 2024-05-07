using Lister.Core.ValueObjects;
using MediatR;

namespace Lister.Application.Queries.Handlers;

public abstract class GetListItemQueryHandlerBase
    : IRequestHandler<GetListItemQuery, Item>
{
    public abstract Task<Item> Handle(
        GetListItemQuery request,
        CancellationToken cancellationToken);
}