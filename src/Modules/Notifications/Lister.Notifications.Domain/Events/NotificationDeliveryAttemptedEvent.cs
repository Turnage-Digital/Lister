using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Domain.Events;

public class NotificationDeliveryAttemptedEvent
    : MediatR.INotification
{
    public NotificationDeliveryAttemptedEvent(
        INotification notification,
        NotificationChannel channel,
        DeliveryStatus status
    )
    {
        Notification = notification;
        Channel = channel;
        Status = status;
    }

    public INotification Notification { get; }
    public NotificationChannel Channel { get; }
    public DeliveryStatus Status { get; }
}