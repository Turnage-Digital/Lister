using Lister.Core;
using MediatR;

namespace Lister.Application.Queries.Handlers;

public abstract class GetListItemDefinitionQueryHandlerBase<TList>
    : IRequestHandler<GetListItemDefinitionQuery<TList>, TList?>
    where TList : IReadOnlyList?
{
    public abstract Task<TList?> Handle(
        GetListItemDefinitionQuery<TList> request,
        CancellationToken cancellationToken
    );
}