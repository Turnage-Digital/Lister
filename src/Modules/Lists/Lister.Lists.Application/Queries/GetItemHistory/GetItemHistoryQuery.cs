using Lister.Core.Application;
using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Application.Queries.GetItemHistory;

public record GetItemHistoryQuery(Guid ListId, int ItemId, int Page, int PageSize)
    : RequestBase<HistoryPage<ItemHistoryType>>;