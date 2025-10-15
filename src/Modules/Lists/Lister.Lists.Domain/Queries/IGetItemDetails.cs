using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Queries;

public interface IGetItemDetails
{
    Task<ItemDetails?> GetAsync(Guid listId, int itemId, CancellationToken cancellationToken);
}