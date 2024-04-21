namespace Lister.App;

public abstract class PagedResponseBase<T>(IEnumerable<T> items, long count)
{
    public IEnumerable<T> Items { get; set; } = items;

    public long Count { get; set; } = count;
}