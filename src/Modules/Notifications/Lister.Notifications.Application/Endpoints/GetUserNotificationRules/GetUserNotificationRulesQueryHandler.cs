using Lister.Notifications.Domain.Services;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUserNotificationRules;

public class GetUserNotificationRulesQueryHandler(INotificationRuleQueryService queryService)
    : IRequestHandler<GetUserNotificationRulesQuery, GetUserNotificationRulesResponse>
{
    public async Task<GetUserNotificationRulesResponse> Handle(
        GetUserNotificationRulesQuery request,
        CancellationToken cancellationToken
    )
    {
        var rules = await queryService.GetByUserAsync(request.UserId, request.ListId, cancellationToken);

        var response = new GetUserNotificationRulesResponse();
        response.Rules.AddRange(rules);

        return response;
    }
}