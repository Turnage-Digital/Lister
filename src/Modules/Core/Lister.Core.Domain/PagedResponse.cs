namespace Lister.Core.Domain;

public class PagedResponse<T>
{
    public PagedResponse(IEnumerable<T> items, long count)
    {
        Items = items;
        Count = count;
    }

    public IEnumerable<T> Items { get; set; }

    public long Count { get; set; }
}