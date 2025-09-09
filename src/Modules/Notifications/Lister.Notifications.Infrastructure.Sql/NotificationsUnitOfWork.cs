using Lister.Core.Infrastructure.Sql;
using Lister.Notifications.Domain;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.Infrastructure.Sql.Lister.Notifications.Infrastructure.Sql;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationsUnitOfWork(NotificationsDbContext dbContext)
    : UnitOfWork<NotificationsDbContext>(dbContext), INotificationsUnitOfWork<NotificationRuleDb, NotificationDb>
{
    private readonly NotificationsDbContext _dbContext = dbContext;

    public INotificationRulesStore<NotificationRuleDb> RulesStore => new NotificationRulesStore(_dbContext);
    public INotificationsStore<NotificationDb> NotificationsStore => new NotificationsStore(_dbContext);
}