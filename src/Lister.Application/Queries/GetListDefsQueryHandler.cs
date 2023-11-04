using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListDefsQueryHandler<TListDef> : IRequestHandler<GetListDefsQuery<TListDef>, TListDef[]>
    where TListDef : IReadOnlyListDef
{
    private readonly IGetReadOnlyListDefs<TListDef> _getReadOnlyListDefs;

    public GetListDefsQueryHandler(IGetReadOnlyListDefs<TListDef> getReadOnlyListDefs)
    {
        _getReadOnlyListDefs = getReadOnlyListDefs;
    }

    public async Task<TListDef[]> Handle(
        GetListDefsQuery<TListDef> request,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _getReadOnlyListDefs.GetAsync(request.UserId, cancellationToken);
        return retval;
    }
}