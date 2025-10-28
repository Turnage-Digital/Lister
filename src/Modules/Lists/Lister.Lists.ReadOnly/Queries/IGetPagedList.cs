using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.ReadOnly.Queries;

public interface IGetPagedList
{
    Task<PagedListDto> GetAsync(
        Guid listId,
        int page,
        int pageSize,
        string? field,
        string? sort,
        CancellationToken cancellationToken
    );
}