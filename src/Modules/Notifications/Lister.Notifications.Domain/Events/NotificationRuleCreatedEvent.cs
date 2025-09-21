namespace Lister.Notifications.Domain.Events;

public class NotificationRuleCreatedEvent
    : MediatR.INotification
{
    public NotificationRuleCreatedEvent(INotificationRule rule, string createdBy)
    {
        Rule = rule;
        CreatedBy = createdBy;
    }

    public INotificationRule Rule { get; }
    public string CreatedBy { get; }
}