using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.UpdateNotificationRule;

public class UpdateNotificationRuleCommand : IRequest
{
    public Guid RuleId { get; set; }
    public string UpdatedBy { get; set; } = null!;
    public NotificationTrigger? Trigger { get; set; }
    public NotificationChannel[]? Channels { get; set; }
    public NotificationSchedule? Schedule { get; set; }
    public bool? IsActive { get; set; }
}