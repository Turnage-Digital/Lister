using Lister.Core.SqlDB.Entities;

namespace Lister.Core.SqlDB;

public class ListerUnitOfWork(ListerDbContext dbContext)
    : UnitOfWork<ListerDbContext>(dbContext), IListerUnitOfWork<ListEntity, ItemEntity>
{
    private readonly ListerDbContext _dbContext = dbContext;

    public IListsStore<ListEntity, ItemEntity> ListsStore => new ListsStore(_dbContext);

    public IItemsStore<ItemEntity> ItemsStore => new ItemsStore();
}