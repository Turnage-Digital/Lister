namespace Lister.Notifications.Domain.Views;

public class NotificationRule : IReadOnlyNotificationRule
{
    public object Trigger { get; set; } = null!;
    public object[] Channels { get; set; } = null!;
    public object Schedule { get; set; } = null!;
    public Guid? Id { get; set; }
    public string UserId { get; set; } = null!;
    public Guid ListId { get; set; }
    public bool IsActive { get; set; }
    public string? TemplateId { get; set; }
    // public DateTime CreatedOn { get; set; }
    // public string CreatedBy { get; set; } = null!;
    // public DateTime? UpdatedOn { get; set; }
    // public string? UpdatedBy { get; set; }
    // public string TriggerJson { get; set; } = null!;
    // public string ChannelsJson { get; set; } = null!;
    // public string ScheduleJson { get; set; } = null!;
}