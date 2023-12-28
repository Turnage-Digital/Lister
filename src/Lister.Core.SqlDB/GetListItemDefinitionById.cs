using Lister.Core.SqlDB.Views;
using Lister.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

public class GetListItemDefinitionById : IGetListItemDefinitionById<ListItemDefinitionView>
{
    private readonly ListerDbContext _dbContext;

    public GetListItemDefinitionById(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ListItemDefinitionView> GetAsync(
        string userId,
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Id == id)
            .Select(list => new ListItemDefinitionView
            {
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
                    }).ToArray(),
            }).SingleAsync(cancellationToken: cancellationToken);
        return retval;
    }
}