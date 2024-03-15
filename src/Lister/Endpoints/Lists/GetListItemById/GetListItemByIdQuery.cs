using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItemById;

public class GetListItemByIdQuery : RequestBase<Item>
{
    public GetListItemByIdQuery(string userId, string listId, string id)
    {
        UserId = userId;
        Id = id;
        ListId = listId;
    }

    public string UserId { get; }

    public string ListId { get; }

    public string Id { get; }
}