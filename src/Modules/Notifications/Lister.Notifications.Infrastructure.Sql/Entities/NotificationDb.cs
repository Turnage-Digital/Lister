using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Infrastructure.Sql.ValueObjects;

namespace Lister.Notifications.Infrastructure.Sql.Entities;

public class NotificationDb : IWritableNotification
{
    // JSON columns for complex types
    public string ContentJson { get; set; } = null!;
    public int Priority { get; set; }

    // Navigation properties
    public NotificationRuleDb? NotificationRule { get; set; }

    public ICollection<NotificationDeliveryAttemptDb> DeliveryAttempts { get; set; } =
        new HashSet<NotificationDeliveryAttemptDb>();

    public ICollection<NotificationHistoryEntryDb> History { get; set; } = new HashSet<NotificationHistoryEntryDb>();
    public Guid? Id { get; set; }
    public Guid? NotificationRuleId { get; set; }
    public string UserId { get; set; } = null!;
    public Guid ListId { get; set; }
    public int? ItemId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public DateTime? DeliveredOn { get; set; }
}