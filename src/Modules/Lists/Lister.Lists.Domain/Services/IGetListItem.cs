using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Domain.Services;

public interface IGetListItem
{
    Task<Item?> GetAsync(string userId, Guid listId, int itemId, CancellationToken cancellationToken);
}