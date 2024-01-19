using Lister.Core;
using MediatR;

namespace Lister.Endpoints.Lists.GetListById;

public class GetListByIdQuery<TList> : IRequest<TList>
    where TList : IReadOnlyList
{
    public GetListByIdQuery(
        string userId,
        string id,
        int? pageNumber,
        int? pageSize,
        string? sortColumn,
        string? sortOrder
    )
    {
        Id = id;
        UserId = userId;
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortColumn = sortColumn;
        SortOrder = sortOrder;
    }

    public string UserId { get; }

    public string Id { get; }

    public int? PageNumber { get; }

    public int? PageSize { get; }

    public string? SortColumn { get; set; }

    public string? SortOrder { get; set; }
}