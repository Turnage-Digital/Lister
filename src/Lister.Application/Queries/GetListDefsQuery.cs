using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListDefsQuery<TListDef> : IRequest<TListDef[]>
    where TListDef : IReadOnlyListDef
{
    public GetListDefsQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }
}