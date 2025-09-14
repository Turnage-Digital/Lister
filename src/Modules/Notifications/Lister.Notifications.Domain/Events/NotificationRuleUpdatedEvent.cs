namespace Lister.Notifications.Domain.Events;

public class NotificationRuleUpdatedEvent
    : MediatR.INotification
{
    public NotificationRuleUpdatedEvent(INotificationRule rule, string updatedBy)
    {
        Rule = rule;
        UpdatedBy = updatedBy;
    }

    public INotificationRule Rule { get; }
    public string UpdatedBy { get; }
}