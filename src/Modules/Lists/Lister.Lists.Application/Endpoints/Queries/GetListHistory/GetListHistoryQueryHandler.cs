using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using Lister.Lists.ReadOnly.Queries;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Queries.GetListHistory;

public class GetListHistoryQueryHandler(IGetListHistory historyGetter)
    : IRequestHandler<GetListHistoryQuery, HistoryPage<ListHistoryType>>
{
    public Task<HistoryPage<ListHistoryType>> Handle(
        GetListHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        return historyGetter.GetAsync(
            request.ListId,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}