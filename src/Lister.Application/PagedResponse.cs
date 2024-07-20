namespace Lister.Application;

public class PagedResponse<T>(IEnumerable<T> items, long count)
{
    public IEnumerable<T> Items { get; set; } = items;

    public long Count { get; set; } = count;
}