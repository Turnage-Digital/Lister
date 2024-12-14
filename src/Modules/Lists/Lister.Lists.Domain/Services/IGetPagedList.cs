using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Services;

public interface IGetPagedList
{
    Task<PagedList> GetAsync(
        Guid listId,
        int page,
        int pageSize,
        string? field,
        string? sort,
        CancellationToken cancellationToken);
}