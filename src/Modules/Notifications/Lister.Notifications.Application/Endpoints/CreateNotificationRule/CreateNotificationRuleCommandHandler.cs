using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.CreateNotificationRule;

public class CreateNotificationRuleCommandHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate
)
    : IRequestHandler<CreateNotificationRuleCommand, CreateNotificationRuleResponse>
    where TNotificationRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    public async Task<CreateNotificationRuleResponse> Handle(
        CreateNotificationRuleCommand request,
        CancellationToken cancellationToken
    )
    {
        var rule = await aggregate.CreateNotificationRuleAsync(
            request.UserId!,
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