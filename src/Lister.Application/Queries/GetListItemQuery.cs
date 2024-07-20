using Lister.Core.ValueObjects;

namespace Lister.Application.Queries;

public class GetListItemQuery(string listId, string itemId) : RequestBase<Item?>
{
    public string ListId { get; } = listId;

    public string ItemId { get; } = itemId;
}