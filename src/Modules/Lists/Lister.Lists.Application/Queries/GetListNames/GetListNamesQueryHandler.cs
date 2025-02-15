using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Queries.GetListNames;

public class GetListNamesQueryHandler(IGetListNames listNamesGetter)
    : IRequestHandler<GetListNamesQuery, ListName[]>
{
    public async Task<ListName[]> Handle(
        GetListNamesQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "UserId is null");
        }

        var retval = await listNamesGetter.GetAsync(cancellationToken);
        return retval;
    }
}