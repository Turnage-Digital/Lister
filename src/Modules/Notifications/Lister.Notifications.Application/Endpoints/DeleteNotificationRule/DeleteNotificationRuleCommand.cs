using Lister.Core.Application;

namespace Lister.Notifications.Application.Endpoints.DeleteNotificationRule;

public record DeleteNotificationRuleCommand 
    : RequestBase
{
    public DeleteNotificationRuleCommand(Guid ruleId)
    {
        RuleId = ruleId;
    }

    public Guid RuleId { get; }
}