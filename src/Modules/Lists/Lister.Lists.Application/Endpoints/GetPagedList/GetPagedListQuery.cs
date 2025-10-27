using Lister.Core.Application;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Endpoints.GetPagedList;

public record GetPagedListQuery(Guid ListId, int Page, int PageSize, string? Field, string? Sort)
    : RequestBase<PagedListDto>;
