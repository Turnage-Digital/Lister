using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItems;

public class GetListItemsResponse
{
    public GetListItemsResponse(IEnumerable<Item> items, long count)
    {
        Items = items;
        Count = count;
    }

    public IEnumerable<Item> Items { get; set; }

    public long Count { get; set; }
}