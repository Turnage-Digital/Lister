using Lister.Core.Application;
using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Application.Endpoints.GetListHistory;

public record GetListHistoryQuery(Guid ListId, int Page, int PageSize)
    : RequestBase<HistoryPage<ListHistoryType>>;