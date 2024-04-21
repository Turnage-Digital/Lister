using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItems;

public class GetListItemsResponse(IEnumerable<Item> items, long count)
{
    public IEnumerable<Item> Items { get; set; } = items;

    public long Count { get; set; } = count;
}