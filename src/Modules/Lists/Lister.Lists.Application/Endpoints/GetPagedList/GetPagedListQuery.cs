using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Endpoints.GetPagedList;

public record GetPagedListQuery(string ListId, int Page, int PageSize, string? Field, string? Sort)
    : RequestBase<PagedList>;