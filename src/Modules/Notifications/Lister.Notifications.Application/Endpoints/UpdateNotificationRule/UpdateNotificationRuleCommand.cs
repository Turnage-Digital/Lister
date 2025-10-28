using Lister.Core.Application;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Application.Endpoints.UpdateNotificationRule;

public record UpdateNotificationRuleCommand(
    Guid RuleId,
    NotificationTrigger Trigger,
    NotificationChannel[] Channels,
    NotificationSchedule Schedule,
    string? TemplateId,
    bool IsActive
) : RequestBase;