namespace Lister.Notifications.Application.Endpoints.GetUserNotificationRules;

public class NotificationRuleDto
{
    public Guid Id { get; set; }
    public Guid ListId { get; set; }
    public bool IsActive { get; set; }
    public object Trigger { get; set; } = null!;
    public object[] Channels { get; set; } = null!;
    public object Schedule { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}