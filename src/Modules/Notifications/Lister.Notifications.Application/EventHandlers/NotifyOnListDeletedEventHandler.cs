using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers;

public class NotifyOnListDeletedEventHandler : INotificationHandler<ListDeletedEvent>
{
    private readonly NotificationAggregate<NotificationRuleDb, NotificationDb> _aggregate;

    public NotifyOnListDeletedEventHandler(NotificationAggregate<NotificationRuleDb, NotificationDb> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task Handle(ListDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.List.Id == null) return;

        var rules = await _aggregate.GetActiveRulesForListAsync(
            notification.List.Id.Value,
            TriggerType.ListDeleted,
            cancellationToken);

        foreach (var rule in rules)
        {
            var trigger = NotificationTrigger.ListDeleted();
            var context = new Dictionary<string, object>
            {
                ["ListId"] = notification.List.Id.Value,
                ["DeletedBy"] = notification.DeletedBy
            };

            if (await _aggregate.ShouldTriggerNotificationAsync(rule, trigger, context, cancellationToken))
            {
                var content = new NotificationContent
                {
                    Subject = "List Deleted",
                    Body = $"The list '{notification.List.Name}' was deleted by {notification.DeletedBy}",
                    ListName = notification.List.Name,
                    TriggeringUser = notification.DeletedBy,
                    OccurredOn = DateTime.UtcNow
                };

                await _aggregate.CreateNotificationAsync(
                    rule.UserId,
                    notification.List.Id.Value,
                    null,
                    rule.Id,
                    content,
                    NotificationPriority.Critical,
                    cancellationToken);
            }
        }
    }
}