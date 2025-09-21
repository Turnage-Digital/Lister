using Lister.Notifications.Domain.Views;

namespace Lister.Notifications.Application.Endpoints.GetUserNotificationRules;

public class GetUserNotificationRulesResponse
{
    public List<NotificationRule> Rules { get; set; } = new();
}