namespace Lister.Notifications.Application.Endpoints.
    GetUserNotificationRules;

public class GetUserNotificationRulesResponse
{
    public List<NotificationRuleDto> Rules { get; set; } = new();
}