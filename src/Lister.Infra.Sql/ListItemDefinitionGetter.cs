using Lister.Domain;
using Lister.Domain.ValueObjects;
using Lister.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Infra.Sql;

public class ListItemDefinitionGetter(ListerDbContext dbContext) : IGetListItemDefinition
{
    public async Task<ListItemDefinition?> Get(string userId, Guid listId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Id == listId)
            .Select(list => new ListItemDefinition
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
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}