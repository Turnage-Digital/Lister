using AutoMapper;
using Lister.Core.SqlDB.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

public class GetReadOnlyListDefById : IGetReadOnlyListDefById<ListDefView>
{
    private readonly ListerDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetReadOnlyListDefById(ListerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ListDefView> GetByIdAsync(string userId, Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ThingDefs
            .Where(thingDef => thingDef.CreatedBy == userId)
            .Where(thingDef => thingDef.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        var retval = _mapper.Map<ListDefView>(entity);
        return retval;
    }
}