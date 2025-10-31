using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationRuleDeletedEvent
    : INotification
{
    public NotificationRuleDeletedEvent(IWritableNotificationRule rule, string deletedBy)
    {
        Rule = rule;
        DeletedBy = deletedBy;
    }

    public IWritableNotificationRule Rule { get; }
    public string DeletedBy { get; }
}