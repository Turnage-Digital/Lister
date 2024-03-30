namespace Lister.Endpoints.Lists.GetListItems;

public class GetListItemsQuery : RequestBase<GetListItemsResponse>
{
    public GetListItemsQuery(
        string listId,
        int? page,
        int? pageSize,
        string? field,
        string? sort
    )
    {
        ListId = listId;
        Page = page;
        PageSize = pageSize;
        Field = field;
        Sort = sort;
    }

    public string ListId { get; }

    public int? Page { get; }

    public int? PageSize { get; }

    public string? Field { get; }

    public string? Sort { get; }
}