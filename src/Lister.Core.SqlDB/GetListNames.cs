using AutoMapper;
using Lister.Core.SqlDB.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

public class GetListNames : IGetListNames<ListNameView>
{
    private readonly ListerDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetListNames(ListerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ListNameView[]> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Select(list => new ListNameView
            {
                Id = list.Id,
                Name = list.Name
            })
            .ToArrayAsync(cancellationToken);
        var retval = _mapper.Map<ListNameView[]>(entities);
        return retval;
    }
}