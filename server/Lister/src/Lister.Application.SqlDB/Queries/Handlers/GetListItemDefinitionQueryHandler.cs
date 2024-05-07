using Lister.Application.Queries;
using Lister.Application.Queries.Handlers;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Views;
using Lister.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Application.SqlDB.Queries.Handlers;

public class GetListItemDefinitionQueryHandler(ListerDbContext dbContext)
    : GetListItemDefinitionQueryHandlerBase<ListItemDefinitionView>
{
    public override async Task<ListItemDefinitionView> Handle(
        GetListItemDefinitionQuery<ListItemDefinitionView> request,
        CancellationToken cancellationToken
    )
    {
        var parsed = Guid.Parse(request.ListId);
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == request.UserId)
            .Where(list => list.Id == parsed)
            .Select(list => new ListItemDefinitionView
            {
                Id = list.Id,
                Name = list.Name,
                Columns = list.Columns
                    .Select(column => new Column
                    {
                        Name = column.Name,
                        Type = column.Type
                    }).ToArray(),
                Statuses = list.Statuses
                    .Select(status => new Status
                    {
                        Name = status.Name,
                        Color = status.Color
                    }).ToArray()
            })
            .AsSplitQuery()
            .SingleAsync(cancellationToken);
        return retval;
    }
}