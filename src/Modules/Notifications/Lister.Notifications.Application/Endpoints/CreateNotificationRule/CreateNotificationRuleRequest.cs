using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public class CreateNotificationRuleRequest
{
    public string UserId { get; set; } = null!;
    public string ListId { get; set; } = null!;
    public NotificationTrigger Trigger { get; set; } = null!;
    public NotificationChannel[] Channels { get; set; } = null!;
    public NotificationSchedule Schedule { get; set; } = null!;
    public string? TemplateId { get; set; }
}