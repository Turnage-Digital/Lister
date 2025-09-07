namespace Lister.Notifications.Domain.Enums;

public enum DeliveryStatus
{
    Pending,
    Queued,
    Sending,
    Delivered,
    Failed,
    Bounced,
    Retry,
    Cancelled
}