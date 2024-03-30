using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItem;

public class GetListItemQuery : RequestBase<Item>
{
    public GetListItemQuery(string listId, string itemId)
    {
        ItemId = itemId;
        ListId = listId;
    }

    public string ListId { get; }

    public string ItemId { get; }
}