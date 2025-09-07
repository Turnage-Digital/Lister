using MediatR;

namespace Lister.Notifications.Application.Endpoints.DeleteNotificationRule;

public class DeleteNotificationRuleCommand : IRequest
{
    public Guid RuleId { get; set; }
    public string DeletedBy { get; set; } = null!;
}