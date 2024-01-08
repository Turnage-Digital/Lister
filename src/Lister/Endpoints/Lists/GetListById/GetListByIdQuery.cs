using Lister.Core;
using MediatR;

namespace Lister.Endpoints.Lists.GetListById;

public class GetListByIdQuery<TList> : IRequest<TList>
    where TList : IReadOnlyList
{
    public GetListByIdQuery(string userId, string id)
    {
        Id = id;
        UserId = userId;
    }

    public string UserId { get; }

    public string Id { get; }
}