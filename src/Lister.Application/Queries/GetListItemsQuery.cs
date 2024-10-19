using Lister.Domain;
using Lister.Domain.Entities;

namespace Lister.Application.Queries.List;

public record GetListItemsQuery(string ListId, int Page, int PageSize, string? Field, string? Sort)
    : RequestBase<PagedResponse<Item>>;