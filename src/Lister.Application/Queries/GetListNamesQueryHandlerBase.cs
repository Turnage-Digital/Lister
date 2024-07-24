using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public abstract class GetListNamesQueryHandlerBase<TList>
    : IRequestHandler<GetListNamesQuery<TList>, TList[]>
    where TList : IReadOnlyList
{
    public abstract Task<TList[]> Handle(
        GetListNamesQuery<TList> request,
        CancellationToken cancellationToken
    );
}