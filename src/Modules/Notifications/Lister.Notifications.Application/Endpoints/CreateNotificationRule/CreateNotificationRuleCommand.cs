using Lister.Core.Application;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public record CreateNotificationRuleCommand(
    Guid ListId,
    NotificationTrigger Trigger,
    NotificationChannel[] Channels,
    NotificationSchedule Schedule,
    string? TemplateId
) : RequestBase<CreateNotificationRuleResponse>;