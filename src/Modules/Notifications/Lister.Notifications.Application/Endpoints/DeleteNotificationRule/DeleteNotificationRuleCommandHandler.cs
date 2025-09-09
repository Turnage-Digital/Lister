using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Infrastructure.Sql.Services;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.DeleteNotificationRule;

public class
    DeleteNotificationRuleCommandHandler<TNotificationRule, TNotification>(
        NotificationAggregate<TNotificationRule, TNotification> aggregate,
        INotificationRuleQueryService queryService
    )
    : IRequestHandler<
        DeleteNotificationRuleCommand>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    public async Task Handle(DeleteNotificationRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await queryService.GetByIdForUpdateAsync(request.RuleId, cancellationToken) as TNotificationRule;
        if (rule == null)
        {
            throw new InvalidOperationException($"Notification rule with id {request.RuleId} does not exist");
        }

        await aggregate.DeleteNotificationRuleAsync(rule, "system", cancellationToken);
    }
}