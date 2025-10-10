using Lister.Core.Application;

namespace Lister.Notifications.Application.Endpoints.DeleteNotificationRule;

public record DeleteNotificationRuleCommand(Guid RuleId) : RequestBase;