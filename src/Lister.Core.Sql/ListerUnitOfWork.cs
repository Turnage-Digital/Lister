using Lister.Core.Sql.Entities;

namespace Lister.Core.Sql;

public class ListerUnitOfWork(ListerDbContext dbContext)
    : UnitOfWork<ListerDbContext>(dbContext), IListerUnitOfWork<ListDb, ItemDb>
{
    private readonly ListerDbContext _dbContext = dbContext;

    public IListsStore<ListDb, ItemDb> ListsStore => new ListsStore(_dbContext);

    public IItemsStore<ItemDb> ItemsStore => new ItemsStore();
}