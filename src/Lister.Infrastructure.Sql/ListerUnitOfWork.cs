using Lister.Domain;
using Lister.Infrastructure.Sql.Entities;

namespace Lister.Infrastructure.Sql;

public class ListerUnitOfWork(ListerDbContext dbContext)
    : UnitOfWork<ListerDbContext>(dbContext), IListerUnitOfWork<ListDb>
{
    private readonly ListerDbContext _dbContext = dbContext;

    public IListsStore<ListDb> ListsStore => new ListsStore(_dbContext);
}