using Lister.Core.Application;

namespace Lister.Notifications.Application.Endpoints.Commands.DeleteNotificationRule;

public record DeleteNotificationRuleCommand(Guid RuleId) : RequestBase;