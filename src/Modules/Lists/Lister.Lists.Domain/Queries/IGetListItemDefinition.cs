using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Queries;

public interface IGetListItemDefinition
{
    Task<ListItemDefinition?> GetAsync(Guid listId, CancellationToken cancellationToken);
}