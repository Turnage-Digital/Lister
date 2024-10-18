using Lister.Domain.Entities;

namespace Lister.Domain;

public interface IGetListItem
{
    Task<Item?> Get(string userId, Guid listId, int itemId, CancellationToken cancellationToken);
}