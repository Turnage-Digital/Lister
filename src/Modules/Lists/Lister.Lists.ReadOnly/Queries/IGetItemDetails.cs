using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.ReadOnly.Queries;

public interface IGetItemDetails
{
    Task<ItemDetailsDto?> GetAsync(Guid listId, int itemId, CancellationToken cancellationToken);
}
