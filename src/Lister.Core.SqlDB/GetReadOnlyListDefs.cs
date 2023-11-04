using AutoMapper;
using Lister.Core.SqlDB.Views;
using Microsoft.EntityFrameworkCore;

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
        var entities = await _dbContext.ListDefs
            .Include(listDef => listDef.StatusDefs)
            .Include(listDef => listDef.ColumnDefs)
            .Where(listDef => listDef.CreatedBy == userId)
            .AsSplitQuery()
            .ToArrayAsync(cancellationToken);
        var retval = _mapper.Map<ListDefView[]>(entities);
        return retval;
    }
}