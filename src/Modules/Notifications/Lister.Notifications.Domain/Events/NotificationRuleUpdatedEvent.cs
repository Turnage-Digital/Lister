using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationRuleUpdatedEvent
    : INotification
{
    public NotificationRuleUpdatedEvent(IWritableNotificationRule rule, string updatedBy)
    {
        Rule = rule;
        UpdatedBy = updatedBy;
    }

    public IWritableNotificationRule Rule { get; }
    public string UpdatedBy { get; }
}