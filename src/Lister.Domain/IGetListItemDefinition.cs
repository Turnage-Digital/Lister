using Lister.Domain.Views;

namespace Lister.Domain;

public interface IGetListItemDefinition
{
    Task<ListItemDefinition?> Get(string userId, Guid listId, CancellationToken cancellationToken);
}