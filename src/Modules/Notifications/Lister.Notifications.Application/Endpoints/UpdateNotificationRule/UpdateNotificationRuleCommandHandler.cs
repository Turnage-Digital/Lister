using Lister.Notifications.Domain;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.UpdateNotificationRule;

public class UpdateNotificationRuleCommandHandler : IRequestHandler<UpdateNotificationRuleCommand>
{
    private readonly NotificationAggregate<NotificationRuleDb, NotificationDb> _aggregate;

    public UpdateNotificationRuleCommandHandler(NotificationAggregate<NotificationRuleDb, NotificationDb> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task Handle(UpdateNotificationRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _aggregate.GetNotificationRuleByIdAsync(request.RuleId, cancellationToken);
        if (rule == null)
        {
            throw new InvalidOperationException($"Notification rule with id {request.RuleId} does not exist");
        }

        await _aggregate.UpdateNotificationRuleAsync(
            rule,
            request.UpdatedBy,
            request.Trigger,
            request.Channels,
            request.Schedule,
            request.IsActive,
            cancellationToken);
    }
}