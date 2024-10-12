using Lister.Core.Entities;

namespace Lister.Core;

public interface IItemsStore<TItem>
    where TItem : Item
{
    Task<TItem> InitAsync(string createdBy, object bag, CancellationToken cancellationToken);
}