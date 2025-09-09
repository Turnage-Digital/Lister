using Lister.Notifications.Infrastructure.Sql.Entities;

namespace Lister.Notifications.Infrastructure.Sql.ValueObjects;

public record NotificationDeliveryAttemptDb
{
    public int? Id { get; set; }
    public Guid? NotificationId { get; set; }
    public NotificationDb? Notification { get; set; }

    public DateTime AttemptedOn { get; set; }
    public string ChannelJson { get; set; } = null!;
    public int Status { get; set; }
    public string? FailureReason { get; set; }
    public int AttemptNumber { get; set; }
    public TimeSpan? NextRetryAfter { get; set; }
}