using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationDeliveryAttemptedEvent
    : INotification
{
    public NotificationDeliveryAttemptedEvent(
        Entities.INotification notification,
        NotificationChannel channel,
        DeliveryStatus status
    )
    {
        Notification = notification;
        Channel = channel;
        Status = status;
    }

    public Entities.INotification Notification { get; }
    public NotificationChannel Channel { get; }
    public DeliveryStatus Status { get; }
}