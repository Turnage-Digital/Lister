using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.UpdateNotificationRule;

public class UpdateNotificationRuleCommand : IRequest
{
    public UpdateNotificationRuleCommand(
        Guid ruleId,
        NotificationTrigger trigger,
        NotificationChannel[] channels,
        NotificationSchedule schedule,
        string? templateId
    )
    {
        RuleId = ruleId;
        Trigger = trigger;
        Channels = channels;
        Schedule = schedule;
        TemplateId = templateId;
    }

    public Guid RuleId { get; }
    public NotificationTrigger Trigger { get; }
    public NotificationChannel[] Channels { get; }
    public NotificationSchedule Schedule { get; }
    public string? TemplateId { get; }
}