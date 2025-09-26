using Lister.Core.Application;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public record CreateNotificationRuleCommand(
    Guid ListId,
    NotificationTrigger Trigger,
    NotificationChannel[] Channels,
    NotificationSchedule Schedule,
    string? TemplateId
) : RequestBase<NotificationRule>;