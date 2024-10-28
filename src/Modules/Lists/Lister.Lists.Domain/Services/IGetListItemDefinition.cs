using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Services;

public interface IGetListItemDefinition
{
    Task<ListItemDefinition?> Get(string userId, Guid listId, CancellationToken cancellationToken);
}