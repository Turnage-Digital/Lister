using Lister.Core.Domain;
using Lister.Core.Infrastructure.Sql;
using Lister.Notifications.Domain;
using Lister.Notifications.Infrastructure.Sql.Entities;
using MediatR;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationsUnitOfWork(NotificationsDbContext dbContext, IMediator mediator, IDomainEventQueue eventQueue)
    : UnitOfWork<NotificationsDbContext>(dbContext, mediator, eventQueue),
        INotificationsUnitOfWork<NotificationRuleDb, NotificationDb>
{
    private readonly NotificationsDbContext _dbContext = dbContext;

    public INotificationRulesStore<NotificationRuleDb> RulesStore => new NotificationRulesStore(_dbContext);
    public INotificationsStore<NotificationDb> NotificationsStore => new NotificationsStore(_dbContext);
}