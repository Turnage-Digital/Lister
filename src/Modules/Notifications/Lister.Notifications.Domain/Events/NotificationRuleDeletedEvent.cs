using Lister.Notifications.Domain.Entities;
using INotification = MediatR.INotification;

namespace Lister.Notifications.Domain.Events;

public class NotificationRuleDeletedEvent
    : INotification
{
    public NotificationRuleDeletedEvent(INotificationRule rule, string deletedBy)
    {
        Rule = rule;
        DeletedBy = deletedBy;
    }

    public INotificationRule Rule { get; }
    public string DeletedBy { get; }
}