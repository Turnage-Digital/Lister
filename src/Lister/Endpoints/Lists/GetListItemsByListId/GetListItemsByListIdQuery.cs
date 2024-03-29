namespace Lister.Endpoints.Lists.GetListItemsByListId;

public class GetListItemsByListIdQuery : RequestBase<GetListItemsByListIdResponse>
{
    public GetListItemsByListIdQuery(
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