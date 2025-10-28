using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public class CreateNotificationRuleRequest
{
    public Guid ListId { get; set; }
    public NotificationTrigger Trigger { get; set; } = null!;
    public NotificationChannel[] Channels { get; set; } = null!;
    public NotificationSchedule Schedule { get; set; } = null!;
    public string? TemplateId { get; set; }
    public bool IsActive { get; set; } = true;
}