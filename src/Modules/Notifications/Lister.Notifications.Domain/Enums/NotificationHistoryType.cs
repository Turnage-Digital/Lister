namespace Lister.Notifications.Domain.Enums;

public enum NotificationHistoryType
{
    Created,
    Updated,
    Processed,
    Delivered,
    Failed,
    Retried,
    Cancelled
}