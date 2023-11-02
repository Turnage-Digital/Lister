using Lister.Core.SqlDB.Entities;

namespace Lister.Core.SqlDB;

public class ListerUnitOfWork : UnitOfWork<ListerDbContext>, IListerUnitOfWork<ListDefEntity>
{
    private readonly ListerDbContext _dbContext;

    public ListerUnitOfWork(ListerDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public IListDefsStore<ListDefEntity> ListDefsStore => new ListDefsStore(_dbContext);
}