using Lister.Application.Queries;
using Lister.Core.Sql;
using Lister.Core.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Application.Sql.Queries;

public class GetListNamesQueryHandler(ListerDbContext dbContext) : GetListNamesQueryHandlerBase<ListName>
{
    public override async Task<ListName[]> Handle(
        GetListNamesQuery<ListName> request,
        CancellationToken cancellationToken
    )
    {
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == request.UserId)
            .Select(list => new ListName
            {
                Id = list.Id,
                Name = list.Name
            })
            .ToArrayAsync(cancellationToken);
        return retval;
    }
}