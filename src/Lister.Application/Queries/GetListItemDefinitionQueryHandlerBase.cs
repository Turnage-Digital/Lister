using Lister.Core;
using Lister.Core.Entities;
using MediatR;

namespace Lister.Application.Queries;

public abstract class GetListItemDefinitionQueryHandlerBase<TList>
    : IRequestHandler<GetListItemDefinitionQuery<TList>, TList?>
    where TList : IReadOnlyList?
{
    public abstract Task<TList?> Handle(
        GetListItemDefinitionQuery<TList> request,
        CancellationToken cancellationToken
    );
}