using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Domain.Services;

public interface IGetItem
{
    Task<Item?> GetAsync(Guid listId, int itemId, CancellationToken cancellationToken);
}