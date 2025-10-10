using Lister.Core.Application;
using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Application.Endpoints.
    GetUserNotificationRules;

public record GetUserNotificationRulesQuery : RequestBase<NotificationRule[]>
{
    public Guid? ListId { get; set; }
}