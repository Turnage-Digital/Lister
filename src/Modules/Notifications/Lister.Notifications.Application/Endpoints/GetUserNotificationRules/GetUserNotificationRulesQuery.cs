using MediatR;

namespace Lister.Notifications.Application.Endpoints.
    GetUserNotificationRules;

public class GetUserNotificationRulesQuery : IRequest<GetUserNotificationRulesResponse>
{
    public string UserId { get; set; } = null!;
    public Guid? ListId { get; set; }
}