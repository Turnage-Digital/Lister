using Lister.Core.Domain.ValueObjects;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.Domain.ValueObjects;

public record NotificationDeliveryAttempt : Entry<DeliveryStatus>
{
    public NotificationChannel Channel { get; init; } = null!;
    public string? FailureReason { get; init; }
    public int AttemptNumber { get; init; }
    public TimeSpan? NextRetryAfter { get; init; }
}