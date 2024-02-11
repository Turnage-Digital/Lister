using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItemsByListId;

public class GetListItemsByListIdResponse
{
    public GetListItemsByListIdResponse(IEnumerable<Item> items, long count)
    {
        Items = items;
        Count = count;
    }

    public IEnumerable<Item> Items { get; set; }

    public long Count { get; set; }
}