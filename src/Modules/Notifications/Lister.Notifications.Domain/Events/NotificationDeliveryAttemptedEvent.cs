using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationDeliveryAttemptedEvent
    : INotification
{
    public NotificationDeliveryAttemptedEvent(
        IWritableNotification notification,
        NotificationChannel channel,
        DeliveryStatus status
    )
    {
        Notification = notification;
        Channel = channel;
        Status = status;
    }

    public IWritableNotification Notification { get; }
    public NotificationChannel Channel { get; }
    public DeliveryStatus Status { get; }
}