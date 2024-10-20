using Lister.Core.Domain;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Domain;

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