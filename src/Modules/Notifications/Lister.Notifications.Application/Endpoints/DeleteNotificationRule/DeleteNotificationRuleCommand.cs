using MediatR;

namespace Lister.Notifications.Application.Endpoints.DeleteNotificationRule;

public class DeleteNotificationRuleCommand : IRequest
{
    public DeleteNotificationRuleCommand(Guid ruleId)
    {
        RuleId = ruleId;
    }

    public Guid RuleId { get; }
}