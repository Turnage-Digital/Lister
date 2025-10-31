using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationRuleCreatedEvent
    : INotification
{
    public NotificationRuleCreatedEvent(IWritableNotificationRule rule, string createdBy)
    {
        Rule = rule;
        CreatedBy = createdBy;
    }

    public IWritableNotificationRule Rule { get; }
    public string CreatedBy { get; }
}