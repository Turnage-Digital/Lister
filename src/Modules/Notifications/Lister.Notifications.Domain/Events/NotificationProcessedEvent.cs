using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationProcessedEvent : INotification
{
    public NotificationProcessedEvent(Entities.INotification notification)
    {
        Notification = notification;
    }

    public Entities.INotification Notification { get; }
}