using Lister.Notifications.Domain;
using MediatR;

namespace Lister.Notifications.Application.Endpoints.DeleteNotificationRule;

public class DeleteNotificationRuleCommandHandler : IRequestHandler<DeleteNotificationRuleCommand>
{
    private readonly NotificationAggregate<NotificationRuleDb, NotificationDb> _aggregate;

    public DeleteNotificationRuleCommandHandler(NotificationAggregate<NotificationRuleDb, NotificationDb> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task Handle(DeleteNotificationRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _aggregate.GetNotificationRuleByIdAsync(request.RuleId, cancellationToken);
        if (rule == null)
        {
            throw new InvalidOperationException($"Notification rule with id {request.RuleId} does not exist");
        }

        await _aggregate.DeleteNotificationRuleAsync(rule, request.DeletedBy, cancellationToken);
    }
}