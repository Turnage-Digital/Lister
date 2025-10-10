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
                        Type = column.Type,
                        Required = column.Required,
                        AllowedValues = column.AllowedValues,
                        MinNumber = column.MinNumber,
                        MaxNumber = column.MaxNumber,
                        Regex = column.Regex
                    })
                    .ToArray(),
                Statuses = list.Statuses
                    .Select(status => new Status
                    {
                        Name = status.Name,
                        Color = status.Color
                    })
                    .ToArray(),
                Transitions = list.StatusTransitions
                    .GroupBy(st => st.From)
                    .Select(g => new StatusTransition
                    {
                        From = g.Key,
                        AllowedNext = g.Select(x => x.To).ToArray()
                    })
                    .ToArray()
            })
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }
}