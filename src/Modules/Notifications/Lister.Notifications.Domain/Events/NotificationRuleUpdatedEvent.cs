using Lister.Notifications.Domain.Entities;
using INotification = MediatR.INotification;

namespace Lister.Notifications.Domain.Events;

public class NotificationRuleUpdatedEvent
    : INotification
{
    public NotificationRuleUpdatedEvent(INotificationRule rule, string updatedBy)
    {
        Rule = rule;
        UpdatedBy = updatedBy;
    }

    public INotificationRule Rule { get; }
    public string UpdatedBy { get; }
}