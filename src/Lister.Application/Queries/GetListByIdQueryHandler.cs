using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListByIdQueryHandler<TList> : IRequestHandler<GetListByIdQuery<TList>, TList>
    where TList : IReadOnlyList
{
    private readonly IGetListById<TList> _getListById;

    public GetListByIdQueryHandler(IGetListById<TList> getListById)
    {
        _getListById = getListById;
    }

    public async Task<TList> Handle(
        GetListByIdQuery<TList> request,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _getListById.GetAsync(request.UserId, request.Id, cancellationToken);
        return retval;
    }
}