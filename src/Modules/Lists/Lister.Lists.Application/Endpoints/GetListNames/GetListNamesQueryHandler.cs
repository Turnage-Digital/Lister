using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetListNames;

public class GetListNamesQueryHandler(IGetListNames listNamesGetter)
    : IRequestHandler<GetListNamesQuery, ListName[]>
{
    public async Task<ListName[]> Handle(
        GetListNamesQuery request,
        CancellationToken cancellationToken
    )
    {
        var retval = await listNamesGetter.GetAsync(cancellationToken);
        return retval;
    }
}