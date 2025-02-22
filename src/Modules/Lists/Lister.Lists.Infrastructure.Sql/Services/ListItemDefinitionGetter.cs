using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListItemDefinitionGetter(ListsDbContext dbContext) : IGetListItemDefinition
{
    public async Task<ListItemDefinition?> GetAsync(Guid listId, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Where(list => list.Id == listId)
            .Where(list => list.IsDeleted == false)
            .Select(list => new ListItemDefinition
            {
                Id = list.Id,
                Name = list.Name,
                Columns = list.Columns
                    .Select(column => new Column
                    {
                        Name = column.Name,
                        Type = column.Type
                    })
                    .ToArray(),
                Statuses = list.Statuses
                    .Select(status => new Status
                    {
                        Name = status.Name,
                        Color = status.Color
                    })
                    .ToArray()
            })
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}