namespace Lister.Notifications.Domain.Events;

public class NotificationProcessedEvent
    : MediatR.INotification
{
    public NotificationProcessedEvent(INotification notification)
    {
        Notification = notification;
    }

    public INotification Notification { get; }
}