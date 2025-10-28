using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationReadEvent
    : INotification
{
    public NotificationReadEvent(IWritableNotification notification)
    {
        Notification = notification;
    }

    public IWritableNotification Notification { get; }
}