using System.Text.Json;
using Lister.Notifications.Domain;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.GetUserNotificationRules;

public class
    GetUserNotificationRulesQueryHandler : IRequestHandler<GetUserNotificationRulesQuery,
    GetUserNotificationRulesResponse>
{
    private readonly NotificationAggregate<NotificationRuleDb, NotificationDb> _aggregate;

    public GetUserNotificationRulesQueryHandler(NotificationAggregate<NotificationRuleDb, NotificationDb> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task<GetUserNotificationRulesResponse> Handle(
        GetUserNotificationRulesQuery request,
        CancellationToken cancellationToken
    )
    {
        var rules = await _aggregate.GetUserNotificationRulesAsync(request.UserId, request.ListId, cancellationToken);

        var response = new GetUserNotificationRulesResponse();

        foreach (var rule in rules)
        {
            response.Rules.Add(new NotificationRuleDto
            {
                Id = rule.Id!.Value,
                ListId = rule.ListId,
                IsActive = rule.IsActive,
                Trigger = JsonSerializer.Deserialize<object>(rule.TriggerJson)!,
                Channels = JsonSerializer.Deserialize<object[]>(rule.ChannelsJson)!,
                Schedule = JsonSerializer.Deserialize<object>(rule.ScheduleJson)!,
                CreatedOn = rule.CreatedOn
            });
        }

        return response;
    }
}