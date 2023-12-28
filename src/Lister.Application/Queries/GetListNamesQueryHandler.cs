using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListNamesQueryHandler<TList> : IRequestHandler<GetListNamesQuery<TList>, TList[]>
    where TList : IReadOnlyList
{
    private readonly IGetListNames<TList> _getListNames;

    public GetListNamesQueryHandler(IGetListNames<TList> getListNames)
    {
        _getListNames = getListNames;
    }

    public async Task<TList[]> Handle(GetListNamesQuery<TList> request, CancellationToken cancellationToken)
    {
        var retval = await _getListNames.GetAsync(request.UserId, cancellationToken);
        return retval;
    }
}