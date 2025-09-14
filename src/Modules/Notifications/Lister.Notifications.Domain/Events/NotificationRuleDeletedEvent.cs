namespace Lister.Notifications.Domain.Events;

public class NotificationRuleDeletedEvent
    : MediatR.INotification
{
    public NotificationRuleDeletedEvent(INotificationRule rule, string deletedBy)
    {
        Rule = rule;
        DeletedBy = deletedBy;
    }

    public INotificationRule Rule { get; }
    public string DeletedBy { get; }
}