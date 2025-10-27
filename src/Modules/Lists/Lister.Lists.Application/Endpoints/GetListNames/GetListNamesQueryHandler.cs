using Lister.Lists.ReadOnly.Queries;
using Lister.Lists.ReadOnly.Dtos;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetListNames;

public class GetListNamesQueryHandler(IGetListNames listNamesGetter)
    : IRequestHandler<GetListNamesQuery, ListNameDto[]>
{
    public async Task<ListNameDto[]> Handle(
        GetListNamesQuery request,
        CancellationToken cancellationToken
    )
    {
        var retval = await listNamesGetter.GetAsync(cancellationToken);
        return retval;
    }
}
