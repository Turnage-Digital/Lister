using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Infrastructure.Sql.ValueObjects;

namespace Lister.Notifications.Infrastructure.Sql.Entities;

public class NotificationRuleDb : IWritableNotificationRule
{
    public bool IsDeleted { get; set; }

    // Navigation properties
    public ICollection<NotificationDb> Notifications { get; set; } = new HashSet<NotificationDb>();

    public ICollection<NotificationRuleHistoryEntryDb> History { get; set; } =
        new HashSet<NotificationRuleHistoryEntryDb>();

    public string? TemplateId { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }

    // JSON columns for complex types
    public string TriggerJson { get; set; } = null!;
    public string ChannelsJson { get; set; } = null!;
    public string ScheduleJson { get; set; } = null!;
    public int TriggerType { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? Id { get; set; }
    public string UserId { get; set; } = null!;
    public Guid ListId { get; set; }
}