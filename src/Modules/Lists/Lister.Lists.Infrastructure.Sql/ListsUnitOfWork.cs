using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;
using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql;

public class ListsUnitOfWork(ListsDbContext dbContext)
    : UnitOfWork<ListsDbContext>(dbContext), IListsUnitOfWork<ListDb, ItemDb>
{
    private readonly ListsDbContext _dbContext = dbContext;

    public IListsStore<ListDb> ListsStore => new ListsStore(_dbContext);

    public IItemsStore<ItemDb> ItemsStore => new ItemsStore(_dbContext);
}