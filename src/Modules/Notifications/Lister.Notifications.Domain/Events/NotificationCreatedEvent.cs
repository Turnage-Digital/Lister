using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationCreatedEvent
    : INotification
{
    public NotificationCreatedEvent(Entities.INotification notification, string createdBy)
    {
        Notification = notification;
        CreatedBy = createdBy;
    }

    public Entities.INotification Notification { get; }
    public string CreatedBy { get; }
}