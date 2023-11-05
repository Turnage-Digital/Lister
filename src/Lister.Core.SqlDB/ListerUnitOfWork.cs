using Lister.Core.SqlDB.Entities;

namespace Lister.Core.SqlDB;

public class ListerUnitOfWork : UnitOfWork<ListerDbContext>, IListerUnitOfWork<ListEntity>
{
    private readonly ListerDbContext _dbContext;

    public ListerUnitOfWork(ListerDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public IListsStore<ListEntity> ListsStore => new ListsStore(_dbContext);
}