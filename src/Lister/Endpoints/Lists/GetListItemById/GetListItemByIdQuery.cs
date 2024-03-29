using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItemById;

public class GetListItemByIdQuery : RequestBase<Item>
{
    public GetListItemByIdQuery(string listId, string id)
    {
        Id = id;
        ListId = listId;
    }

    public string ListId { get; }

    public string Id { get; }
}