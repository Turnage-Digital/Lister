using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.UpdateNotificationRule;

public class UpdateNotificationRuleCommandHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate
) : IRequestHandler<UpdateNotificationRuleCommand>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    public async Task Handle(UpdateNotificationRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await aggregate.GetNotificationRuleByIdAsync(request.RuleId, cancellationToken);
        if (rule == null)
        {
            throw new InvalidOperationException($"Notification rule with id {request.RuleId} does not exist");
        }

        await aggregate.UpdateNotificationRuleAsync(
            rule,
            request.UserId!,
            request.Trigger,
            request.Channels,
            request.Schedule,
            null, // isActive
            cancellationToken);
    }
}