using MediatR;

namespace Lister.Endpoints.Lists.GetListItemsByListId;

public class GetListItemsByListIdQuery : IRequest<GetListItemsByListIdResponse>
{
    public GetListItemsByListIdQuery(
        string userId,
        string id,
        int? page,
        int? pageSize,
        string? sortColumn,
        string? sortOrder
    )
    {
        Id = id;
        UserId = userId;
        Page = page;
        PageSize = pageSize;
        SortColumn = sortColumn;
        SortOrder = sortOrder;
    }

    public string UserId { get; }

    public string Id { get; }

    public int? Page { get; }

    public int? PageSize { get; }

    public string? SortColumn { get; set; }

    public string? SortOrder { get; set; }
}