using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Services;

public interface IGetListItemDefinition
{
    Task<ListItemDefinition?> GetAsync(string userId, Guid listId, CancellationToken cancellationToken);
}