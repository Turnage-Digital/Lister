using AutoMapper;
using Lister.Core.SqlDB.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

public class GetListById : IGetListById<ListView>
{
    private readonly ListerDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetListById(ListerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ListView> GetAsync(string userId, Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Lists
            .Include(list => list.Statuses)
            .Include(list => list.Columns)
            .Include(list => list.Items)
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Id == id)
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);
        var retval = _mapper.Map<ListView>(entity);
        return retval;
    }
}