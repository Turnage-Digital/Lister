using Lister.Core.ValueObjects;

namespace Lister.Application.Queries;

public class GetListItemsQuery(
    string listId,
    int? page,
    int? pageSize,
    string? field,
    string? sort)
    : RequestBase<PagedResponse<Item>>
{
    public string ListId { get; } = listId;

    public int? Page { get; } = page;

    public int? PageSize { get; } = pageSize;

    public string? Field { get; } = field;

    public string? Sort { get; } = sort;
}