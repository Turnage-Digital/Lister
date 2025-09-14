namespace Lister.Notifications.Domain.Events;

public class NotificationReadEvent<TNotification>
    : MediatR.INotification where TNotification : INotification
{
    public NotificationReadEvent(TNotification notification)
    {
        Notification = notification;
    }

    public TNotification Notification { get; }
}

public class NotificationReadEvent : NotificationReadEvent<INotification>
{
    public NotificationReadEvent(INotification notification)
        : base(notification)
    {
    }
}