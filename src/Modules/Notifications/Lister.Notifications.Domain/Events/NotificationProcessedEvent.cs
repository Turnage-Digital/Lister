using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationProcessedEvent
    : INotification
{
    public NotificationProcessedEvent(IWritableNotification notification)
    {
        Notification = notification;
    }

    public IWritableNotification Notification { get; }
}