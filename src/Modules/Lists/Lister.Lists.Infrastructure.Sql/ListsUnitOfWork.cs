using Lister.Core.Domain;
using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;

namespace Lister.Lists.Infrastructure.Sql;

public class ListsUnitOfWork(ListsDbContext dbContext, IMediator mediator, IDomainEventQueue eventQueue)
    : UnitOfWork<ListsDbContext>(dbContext, mediator, eventQueue), IListsUnitOfWork<ListDb, ItemDb, ListMigrationJobDb>
{
    private readonly ListsDbContext _dbContext = dbContext;

    public IListsStore<ListDb> ListsStore => new ListsStore(_dbContext);

    public IItemsStore<ItemDb> ItemsStore => new ItemsStore(_dbContext);

    public IListMigrationJobStore<ListMigrationJobDb> MigrationJobsStore => new ListMigrationJobStore(_dbContext);
}