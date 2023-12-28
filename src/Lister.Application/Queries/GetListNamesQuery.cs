using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListNamesQuery<TList> : IRequest<TList[]>
    where TList : IReadOnlyList
{
    public GetListNamesQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }
}