using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListDefByIdQueryHandler<TListDef> : IRequestHandler<GetListDefByIdQuery<TListDef>, TListDef>
    where TListDef : IReadOnlyListDef
{
    private readonly IGetReadOnlyListDefById<TListDef> _getReadOnlyListDefById;

    public GetListDefByIdQueryHandler(IGetReadOnlyListDefById<TListDef> getReadOnlyListDefById)
    {
        _getReadOnlyListDefById = getReadOnlyListDefById;
    }

    public async Task<TListDef> Handle(
        GetListDefByIdQuery<TListDef> request,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _getReadOnlyListDefById.GetByIdAsync(request.UserId, request.Id, cancellationToken);
        return retval;
    }
}