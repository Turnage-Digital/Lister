using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Application.Endpoints.UpdateNotificationRule;

public class UpdateNotificationRuleRequest
{
    public NotificationTrigger Trigger { get; set; } = null!;
    public NotificationChannel[] Channels { get; set; } = null!;
    public NotificationSchedule Schedule { get; set; } = null!;
    public string? TemplateId { get; set; }
    public bool IsActive { get; set; } = true;
}