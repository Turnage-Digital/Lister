using Lister.Core;
using MediatR;

namespace Lister.Endpoints.Lists.GetListById;

public class GetListByIdQuery<TList> : IRequest<TList>
    where TList : IReadOnlyList
{
    public GetListByIdQuery(
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