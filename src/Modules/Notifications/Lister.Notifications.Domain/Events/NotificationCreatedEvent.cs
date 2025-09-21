namespace Lister.Notifications.Domain.Events;

public class NotificationCreatedEvent
    : MediatR.INotification
{
    public NotificationCreatedEvent(INotification notification, string createdBy)
    {
        Notification = notification;
        CreatedBy = createdBy;
    }

    public INotification Notification { get; }
    public string CreatedBy { get; }
}