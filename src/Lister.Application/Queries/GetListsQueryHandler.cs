using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListsQueryHandler<TList> : IRequestHandler<GetListsQuery<TList>, TList[]>
    where TList : IReadOnlyList
{
    private readonly IGetLists<TList> _getLists;

    public GetListsQueryHandler(IGetLists<TList> getLists)
    {
        _getLists = getLists;
    }

    public async Task<TList[]> Handle(
        GetListsQuery<TList> request,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _getLists.GetAsync(request.UserId, cancellationToken);
        return retval;
    }
}