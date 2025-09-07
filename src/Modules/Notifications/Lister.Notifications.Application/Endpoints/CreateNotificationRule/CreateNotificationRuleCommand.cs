using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public class CreateNotificationRuleCommand : IRequest<CreateNotificationRuleResponse>
{
    public string UserId { get; set; } = null!;
    public Guid ListId { get; set; }
    public NotificationTrigger Trigger { get; set; } = null!;
    public NotificationChannel[] Channels { get; set; } = null!;
    public NotificationSchedule Schedule { get; set; } = null!;
    public string? TemplateId { get; set; }
}