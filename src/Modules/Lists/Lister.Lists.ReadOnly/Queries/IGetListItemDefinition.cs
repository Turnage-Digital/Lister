using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.ReadOnly.Queries;

public interface IGetListItemDefinition
{
    Task<ListItemDefinitionDto?> GetAsync(Guid listId, CancellationToken cancellationToken);
}