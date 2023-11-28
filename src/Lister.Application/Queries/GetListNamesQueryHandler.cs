using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListNamesQueryHandler<T> : IRequestHandler<GetListNamesQuery<T>, T[]>
{
    private readonly IGetListNames<T> _getListNames;

    public GetListNamesQueryHandler(IGetListNames<T> getListNames)
    {
        _getListNames = getListNames;
    }

    public async Task<T[]> Handle(GetListNamesQuery<T> request, CancellationToken cancellationToken)
    {
        var retval = await _getListNames.GetAsync(request.UserId, cancellationToken);
        return retval;
    }
}