using Lister.Core.Application;
using Lister.Notifications.ReadOnly.Dtos;

namespace Lister.Notifications.Application.Endpoints.Queries.GetUserNotificationRules;

public record GetUserNotificationRulesQuery : RequestBase<NotificationRuleDto[]>
{
    public Guid? ListId { get; set; }
}