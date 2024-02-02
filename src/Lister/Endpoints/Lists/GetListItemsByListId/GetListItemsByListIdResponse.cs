using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItemsByListId;

public class GetListItemsByListIdResponse
{
    public GetListItemsByListIdResponse(IEnumerable<Item> items, int count)
    {
        Items = items;
        Count = count;
    }

    public IEnumerable<Item> Items { get; set; }

    public int Count { get; set; }
}