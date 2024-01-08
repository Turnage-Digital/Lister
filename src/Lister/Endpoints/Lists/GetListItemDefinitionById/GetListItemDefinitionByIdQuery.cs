using Lister.Core;
using MediatR;

namespace Lister.Endpoints.Lists.GetListItemDefinitionById;

public class GetListItemDefinitionByIdQuery<TList> : IRequest<TList>
    where TList : IReadOnlyList
{
    public GetListItemDefinitionByIdQuery(string userId, string id)
    {
        Id = id;
        UserId = userId;
    }

    public string UserId { get; }

    public string Id { get; }
}