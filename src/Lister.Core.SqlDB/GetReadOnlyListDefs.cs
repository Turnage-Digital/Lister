using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Lister.Core.SqlDB.Views;

namespace Lister.Core.SqlDB;

public class GetReadOnlyListDefs : IGetReadOnlyListDefs<ListDefView>
{
    private readonly ListerDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetReadOnlyListDefs(ListerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ListDefView[]> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.ThingDefs
            .Include(thingDef => thingDef.StatusDefs)
            .Include(thingDef => thingDef.ColumnDefs)
            .Where(thingDef => thingDef.CreatedBy == userId)
            .ToArrayAsync(cancellationToken);
        var retval = _mapper.Map<ListDefView[]>(entities);
        return retval;
    }
}