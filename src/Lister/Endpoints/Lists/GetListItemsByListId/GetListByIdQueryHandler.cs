using Lister.Core.SqlDB;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.Endpoints.Lists.GetListItemsByListId;

public class GetListByIdQueryHandler : IRequestHandler<GetListItemsByListIdQuery, GetListItemsByListIdResponse>
{
    private readonly ListerDbContext _dbContext;

    public GetListByIdQueryHandler(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetListItemsByListIdResponse> Handle(
        GetListItemsByListIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var parsed = Guid.Parse(request.Id);
        var page = request.Page ?? 0;
        var pageSize = request.PageSize ?? 10;
        var entities = await _dbContext.Items
            .Where(item => item.ListId == parsed)
            .OrderBy(item => item.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
        var count = await _dbContext.Items
            .CountAsync(item => item.ListId == parsed, cancellationToken);
        var retval = new GetListItemsByListIdResponse(entities, count);
        return retval;
    }
}