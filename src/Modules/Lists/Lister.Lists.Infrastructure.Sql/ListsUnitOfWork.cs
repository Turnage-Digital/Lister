using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;

namespace Lister.Lists.Infrastructure.Sql;

public class ListsUnitOfWork(ListerDbContext dbContext)
    : UnitOfWork<ListerDbContext>(dbContext), IListsUnitOfWork<ListDb>
{
    private readonly ListerDbContext _dbContext = dbContext;

    public IListsStore<ListDb> ListsStore => new ListsStore(_dbContext);
}