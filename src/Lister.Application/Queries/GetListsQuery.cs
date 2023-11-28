using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListsQuery<TList> : IRequest<TList[]>
    where TList : IReadOnlyList
{
    public GetListsQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }
}