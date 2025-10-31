using Lister.Core.Application;

namespace Lister.Notifications.Application.Commands.DeleteNotificationRule;

public record DeleteNotificationRuleCommand(Guid RuleId) : RequestBase;