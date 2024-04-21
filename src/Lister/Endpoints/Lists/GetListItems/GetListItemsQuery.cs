namespace Lister.Endpoints.Lists.GetListItems;

public class GetListItemsQuery(
    string listId,
    int? page,
    int? pageSize,
    string? field,
    string? sort)
    : RequestBase<GetListItemsResponse>
{
    public string ListId { get; } = listId;

    public int? Page { get; } = page;

    public int? PageSize { get; } = pageSize;

    public string? Field { get; } = field;

    public string? Sort { get; } = sort;
}