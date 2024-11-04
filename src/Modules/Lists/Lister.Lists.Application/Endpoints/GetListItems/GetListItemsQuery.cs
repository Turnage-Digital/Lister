using Lister.Core.Application;
using Lister.Core.Domain;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Application.Endpoints.GetListItems;

public record GetListItemsQuery(string ListId, int Page, int PageSize, string? Field, string? Sort)
    : RequestBase<PagedResponse<Item>>;