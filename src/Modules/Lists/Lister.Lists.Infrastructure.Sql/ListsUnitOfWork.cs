using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;
using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql;

public class ListsUnitOfWork(ListerDbContext dbContext)
    : UnitOfWork<ListerDbContext>(dbContext), IListsUnitOfWork<ListDb, ItemDb>
{
    private readonly ListerDbContext _dbContext = dbContext;

    public IListsStore<ListDb> ListsStore => new ListsStore(_dbContext);

    public IItemsStore<ItemDb> ItemsStore => new ItemsStore(_dbContext);
}