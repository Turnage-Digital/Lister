namespace Lister.Notifications.Domain.Events;

public class NotificationReadEvent
    : MediatR.INotification
{
    public NotificationReadEvent(INotification notification)
    {
        Notification = notification;
    }

    public INotification Notification { get; }
}