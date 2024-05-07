using Lister.Core.SqlDB.Entities;

namespace Lister.Core.SqlDB;

public class ListerUnitOfWork(ListerDbContext dbContext)
    : UnitOfWork<ListerDbContext>(dbContext), IListerUnitOfWork<ListEntity>
{
    private readonly ListerDbContext _dbContext = dbContext;

    public IListsStore<ListEntity> ListsStore => new ListsStore(_dbContext);
}