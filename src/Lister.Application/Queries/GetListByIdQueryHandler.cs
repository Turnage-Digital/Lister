using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListByIdQueryHandler<TList> : IRequestHandler<GetListByIdQuery<TList>, TList>
    where TList : IReadOnlyList
{
    private readonly IGetReadOnlyListById<TList> _getReadOnlyListById;

    public GetListByIdQueryHandler(IGetReadOnlyListById<TList> getReadOnlyListById)
    {
        _getReadOnlyListById = getReadOnlyListById;
    }

    public async Task<TList> Handle(
        GetListByIdQuery<TList> request,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _getReadOnlyListById.GetAsync(request.UserId, request.Id, cancellationToken);
        return retval;
    }
}