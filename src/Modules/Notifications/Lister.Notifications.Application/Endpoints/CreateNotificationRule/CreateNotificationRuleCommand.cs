using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public class CreateNotificationRuleCommand : IRequest<CreateNotificationRuleResponse>
{
    public CreateNotificationRuleCommand(
        string userId,
        Guid listId,
        NotificationTrigger trigger,
        NotificationChannel[] channels,
        NotificationSchedule schedule,
        string? templateId
    )
    {
        UserId = userId;
        ListId = listId;
        Trigger = trigger;
        Channels = channels;
        Schedule = schedule;
        TemplateId = templateId;
    }

    public string UserId { get; }
    public Guid ListId { get; }
    public NotificationTrigger Trigger { get; }
    public NotificationChannel[] Channels { get; }
    public NotificationSchedule Schedule { get; }
    public string? TemplateId { get; }
}