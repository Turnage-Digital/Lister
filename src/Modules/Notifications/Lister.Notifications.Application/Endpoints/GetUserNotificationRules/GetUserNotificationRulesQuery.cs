using MediatR;

namespace Lister.Notifications.Application.Endpoints.
    GetUserNotificationRules;

public class GetUserNotificationRulesQuery : IRequest<GetUserNotificationRulesResponse>
{
    public GetUserNotificationRulesQuery(string userId, Guid? listId = null)
    {
        UserId = userId;
        ListId = listId;
    }

    public string UserId { get; }
    public Guid? ListId { get; }
}