using System.Linq;
using Lister.Notifications.ReadOnly.Dtos;
using Lister.Notifications.ReadOnly.Queries;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUserNotificationRules;

public class GetUserNotificationRulesQueryHandler(IGetUserNotificationRules getter)
    : IRequestHandler<GetUserNotificationRulesQuery, NotificationRuleDto[]>
{
    public async Task<NotificationRuleDto[]> Handle(
        GetUserNotificationRulesQuery request,
        CancellationToken cancellationToken
    )
    {
        var rules = await getter.GetAsync(request.UserId!, request.ListId, cancellationToken);
        return rules.ToArray();
    }
}
