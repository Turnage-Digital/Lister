using Lister.Core.Application;
using Lister.Core.Domain;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Application.Queries;

public record GetListItemsQuery(string ListId, int Page, int PageSize, string? Field, string? Sort)
    : RequestBase<PagedResponse<Item>>;