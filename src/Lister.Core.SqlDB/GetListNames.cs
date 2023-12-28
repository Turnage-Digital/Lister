using AutoMapper;
using Lister.Core.SqlDB.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

public class GetListNames : IGetListNames<ListNameView>
{
    private readonly ListerDbContext _dbContext;

    public GetListNames(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ListNameView[]> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var retval = await _dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Select(list => new ListNameView
            {
                Id = list.Id,
                Name = list.Name
            })
            .ToArrayAsync(cancellationToken);
        return retval;
    }
}