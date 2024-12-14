using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Domain;

public interface IItemsStore<TItem>
    where TItem : IWritableItem
{
    Task<TItem?> GetByIdAsync(int itemId, Guid listId, CancellationToken cancellationToken);

    Task<TItem> InitAsync(Guid listId, string createdBy, CancellationToken cancellationToken);

    Task CreateAsync(TItem item, CancellationToken cancellationToken);

    Task DeleteAsync(TItem item, string deletedBy, CancellationToken cancellationToken);

    Task SetBagAsync(TItem item, object bag, CancellationToken cancellationToken);

    Task<object> GetBagAsync(TItem item, CancellationToken cancellationToken);
}