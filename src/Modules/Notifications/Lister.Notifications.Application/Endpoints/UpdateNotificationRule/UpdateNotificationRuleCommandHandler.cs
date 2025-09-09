using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Infrastructure.Sql.Services;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.UpdateNotificationRule;

public class
    UpdateNotificationRuleCommandHandler<TNotificationRule, TNotification> : IRequestHandler<
    UpdateNotificationRuleCommand>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    private readonly NotificationAggregate<TNotificationRule, TNotification> _aggregate;
    private readonly INotificationRuleQueryService _queryService;

    public UpdateNotificationRuleCommandHandler(
        NotificationAggregate<TNotificationRule, TNotification> aggregate,
        INotificationRuleQueryService queryService
    )
    {
        _aggregate = aggregate;
        _queryService = queryService;
    }

    public async Task Handle(UpdateNotificationRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _queryService.GetByIdForUpdateAsync(request.RuleId, cancellationToken) as TNotificationRule;
        if (rule == null)
        {
            throw new InvalidOperationException($"Notification rule with id {request.RuleId} does not exist");
        }

        await _aggregate.UpdateNotificationRuleAsync(
            rule,
            "system", // TODO: Get actual user from context
            request.Trigger,
            request.Channels,
            request.Schedule,
            null, // isActive
            cancellationToken);
    }
}