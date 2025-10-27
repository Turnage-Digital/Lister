using System.Linq;
using Lister.Lists.ReadOnly.Queries;
using Lister.Lists.ReadOnly.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListItemDefinitionGetter(ListsDbContext dbContext) : IGetListItemDefinition
{
    public async Task<ListItemDefinitionDto?> GetAsync(Guid listId, CancellationToken cancellationToken)
    {
        var list = await dbContext.Lists
            .Where(l => l.Id == listId)
            .Where(l => l.IsDeleted == false)
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .Include(l => l.StatusTransitions)
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);

        if (list is null)
        {
            return null;
        }

        var columns = list.Columns
            .Select(column => new ColumnDto
            {
                StorageKey = column.StorageKey,
                Name = column.Name,
                Property = ComputeProperty(column.Name),
                Type = column.Type,
                Required = column.Required,
                AllowedValues = column.AllowedValues,
                MinNumber = column.MinNumber,
                MaxNumber = column.MaxNumber,
                Regex = column.Regex
            })
            .ToArray();

        var statuses = list.Statuses
            .Select(status => new StatusDto
            {
                Name = status.Name,
                Color = status.Color
            })
            .ToArray();

        var transitions = list.StatusTransitions
            .Select(st => new StatusTransitionDto
            {
                From = st.From,
                AllowedNext = st.AllowedNext
            })
            .ToArray();

        var retval = new ListItemDefinitionDto
        {
            Id = list.Id,
            Name = list.Name,
            Columns = columns,
            Statuses = statuses,
            Transitions = transitions
        };
        return retval;

        static string ComputeProperty(string name)
        {
            var nameWithoutSpaces = name.Replace(" ", string.Empty);
            var filtered = new string(nameWithoutSpaces.Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrEmpty(filtered))
            {
                return string.Empty;
            }

            return char.ToLowerInvariant(filtered[0]) + filtered[1..];
        }
    }
}
