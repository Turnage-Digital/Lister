using Lister.Notifications.Domain;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.
    CreateNotificationRule;

public class
    CreateNotificationRuleCommandHandler : IRequestHandler<CreateNotificationRuleCommand,
    CreateNotificationRuleResponse>
{
    private readonly NotificationAggregate<NotificationRuleDb, NotificationDb> _aggregate;

    public CreateNotificationRuleCommandHandler(NotificationAggregate<NotificationRuleDb, NotificationDb> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task<CreateNotificationRuleResponse> Handle(
        CreateNotificationRuleCommand request,
        CancellationToken cancellationToken
    )
    {
        var rule = await _aggregate.CreateNotificationRuleAsync(
            request.UserId,
            request.ListId,
            request.Trigger,
            request.Channels,
            request.Schedule,
            request.TemplateId,
            cancellationToken);

        return new CreateNotificationRuleResponse
        {
            RuleId = rule.Id!.Value
        };
    }
}