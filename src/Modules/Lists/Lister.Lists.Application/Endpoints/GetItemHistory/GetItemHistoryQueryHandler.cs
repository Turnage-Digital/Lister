using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Queries;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetItemHistory;

public class GetItemHistoryQueryHandler(IGetItemHistory historyGetter)
    : IRequestHandler<GetItemHistoryQuery, HistoryPage<ItemHistoryType>>
{
    public Task<HistoryPage<ItemHistoryType>> Handle(
        GetItemHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        return historyGetter.GetAsync(
            request.ListId,
            request.ItemId,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}