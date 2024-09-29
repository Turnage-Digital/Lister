using Lister.Core.ValueObjects;

namespace Lister.Application.Queries;

public record GetListItemsQuery(string ListId, int Page, int PageSize, string? Field, string? Sort)
    : RequestBase<PagedResponse<Item>>;