using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;

namespace Lister.Lists.Infrastructure.Sql;

public class ListerUnitOfWork(ListerDbContext dbContext)
    : UnitOfWork<ListerDbContext>(dbContext), IListerUnitOfWork<ListDb>
{
    private readonly ListerDbContext _dbContext = dbContext;

    public IListsStore<ListDb> ListsStore => new ListsStore(_dbContext);
}