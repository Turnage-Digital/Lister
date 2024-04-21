using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItem;

public class GetListItemQuery(string listId, string itemId) : RequestBase<Item>
{
    public string ListId { get; } = listId;

    public string ItemId { get; } = itemId;
}