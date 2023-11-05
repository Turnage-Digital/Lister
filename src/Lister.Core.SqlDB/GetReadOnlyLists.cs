using AutoMapper;
using Lister.Core.SqlDB.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

public class GetReadOnlyLists : IGetReadOnlyLists<ListView>
{
    private readonly ListerDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetReadOnlyLists(ListerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ListView[]> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Lists
            .Include(list => list.Statuses)
            .Include(list => list.Columns)
            .Where(list => list.CreatedBy == userId)
            .AsSplitQuery()
            .ToArrayAsync(cancellationToken);
        var retval = _mapper.Map<ListView[]>(entities);
        return retval;
    }
}