using Lister.Domain.Entities;

namespace Lister.Domain;

public interface IGetListItems
{
    Task<PagedResponse<Item>> Get(
        string userId,
        Guid listId,
        int page,
        int pageSize,
        string? field,
        string? sort,
        CancellationToken cancellationToken);
}