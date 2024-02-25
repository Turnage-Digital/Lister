using MediatR;

namespace Lister.Endpoints.Lists.GetListItemsByListId;

public class GetListItemsByListIdQuery : RequestBase<GetListItemsByListIdResponse>
{
    public GetListItemsByListIdQuery(
        string userId,
        string listId,
        int? page,
        int? pageSize,
        string? field,
        string? sort
    )
    {
        ListId = listId;
        UserId = userId;
        Page = page;
        PageSize = pageSize;
        Field = field;
        Sort = sort;
    }

    public string UserId { get; }

    public string ListId { get; }

    public int? Page { get; }

    public int? PageSize { get; }

    public string? Field { get; }

    public string? Sort { get; }
}