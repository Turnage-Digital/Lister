using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListDefByIdQuery<TListDef> : IRequest<TListDef>
    where TListDef : IReadOnlyListDef
{
    public GetListDefByIdQuery(string userId, Guid id)
    {
        Id = id;
        UserId = userId;
    }

    public string UserId { get; }

    public Guid Id { get; }
}