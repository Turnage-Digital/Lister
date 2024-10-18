using Lister.Domain;
using Lister.Domain.Views;
using MediatR;

namespace Lister.Application.Queries;

public class GetListNamesQueryHandler(IGetListNames listNamesGetter)
    : IRequestHandler<GetListNamesQuery, ListName[]>
{
    public async Task<ListName[]> Handle(
        GetListNamesQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var retval = await listNamesGetter.Get(request.UserId, cancellationToken);
        return retval;
    }
}