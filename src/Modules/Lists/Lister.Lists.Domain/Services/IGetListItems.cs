using Lister.Core.Domain;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Services;

public interface IGetListItems
{
    Task<PagedResponse<ListItem>> GetAsync(
        Guid listId,
        int page,
        int pageSize,
        string? field,
        string? sort,
        CancellationToken cancellationToken);
}