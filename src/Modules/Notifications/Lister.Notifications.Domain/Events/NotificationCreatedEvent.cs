using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Domain.Events;

public class NotificationCreatedEvent
    : INotification
{
    public NotificationCreatedEvent(IWritableNotification notification, string createdBy)
    {
        Notification = notification;
        CreatedBy = createdBy;
    }

    public IWritableNotification Notification { get; }
    public string CreatedBy { get; }
}