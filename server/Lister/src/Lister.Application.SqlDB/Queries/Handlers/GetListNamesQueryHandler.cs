using Lister.Application.Queries;
using Lister.Application.Queries.Handlers;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Application.SqlDB.Queries.Handlers;

public class GetListNamesQueryHandler(ListerDbContext dbContext) : GetListNamesQueryHandlerBase<ListNameView>
{
    public override async Task<ListNameView[]> Handle(
        GetListNamesQuery<ListNameView> request,
        CancellationToken cancellationToken
    )
    {
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == request.UserId)
            .Select(list => new ListNameView
            {
                Id = list.Id,
                Name = list.Name
            })
            .ToArrayAsync(cancellationToken);
        return retval;
    }
}