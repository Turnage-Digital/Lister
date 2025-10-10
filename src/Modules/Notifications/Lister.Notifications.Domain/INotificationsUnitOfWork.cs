using Lister.Core.Domain;
using Lister.Notifications.Domain.Entities;

namespace Lister.Notifications.Domain;

public interface INotificationsUnitOfWork<TRule, TNotification> : IUnitOfWork
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    INotificationRulesStore<TRule> RulesStore { get; }

    INotificationsStore<TNotification> NotificationsStore { get; }
}