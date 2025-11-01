using Lister.Lists.ReadOnly.Dtos;
using Lister.Lists.ReadOnly.Queries;
using MediatR;

namespace Lister.Lists.Application.Queries.GetListNames;

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