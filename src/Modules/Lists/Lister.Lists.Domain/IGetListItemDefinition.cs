using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain;

public interface IGetListItemDefinition
{
    Task<ListItemDefinition?> Get(string userId, Guid listId, CancellationToken cancellationToken);
}