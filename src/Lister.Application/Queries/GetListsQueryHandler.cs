using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListsQueryHandler<TList> : IRequestHandler<GetListsQuery<TList>, TList[]>
    where TList : IReadOnlyList
{
    private readonly IGetReadOnlyLists<TList> _getReadOnlyLists;

    public GetListsQueryHandler(IGetReadOnlyLists<TList> getReadOnlyLists)
    {
        _getReadOnlyLists = getReadOnlyLists;
    }

    public async Task<TList[]> Handle(
        GetListsQuery<TList> request,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _getReadOnlyLists.GetAsync(request.UserId, cancellationToken);
        return retval;
    }
}