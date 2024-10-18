namespace Lister.Domain;

public interface IItemsStore<TItem>
{
    Task<TItem> InitAsync(string createdBy, object bag, CancellationToken cancellationToken);
}