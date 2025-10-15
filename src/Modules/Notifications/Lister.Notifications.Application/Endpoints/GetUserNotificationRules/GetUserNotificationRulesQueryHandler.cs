using Lister.Notifications.Domain.Queries;
using Lister.Notifications.Domain.Views;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUserNotificationRules;

public class GetUserNotificationRulesQueryHandler(IGetUserNotificationRules getter)
    : IRequestHandler<GetUserNotificationRulesQuery, NotificationRule[]>
{
    public async Task<NotificationRule[]> Handle(
        GetUserNotificationRulesQuery request,
        CancellationToken cancellationToken
    )
    {
        var rules = await getter.GetAsync(request.UserId!, request.ListId, cancellationToken);
        return rules.ToArray();
    }
}