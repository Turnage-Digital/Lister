using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers;

public class NotifyOnItemDeletedEventHandler : INotificationHandler<ListItemDeletedEvent>
{
    private readonly NotificationAggregate<NotificationRuleDb, NotificationDb> _aggregate;

    public NotifyOnItemDeletedEventHandler(NotificationAggregate<NotificationRuleDb, NotificationDb> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task Handle(ListItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Item.ListId == null) return;

        var rules = await _aggregate.GetActiveRulesForListAsync(
            notification.Item.ListId.Value,
            TriggerType.ItemDeleted,
            cancellationToken);

        foreach (var rule in rules)
        {
            var trigger = NotificationTrigger.ItemDeleted();
            var context = new Dictionary<string, object>
            {
                ["ItemId"] = notification.Item.Id!,
                ["DeletedBy"] = notification.DeletedBy
            };

            if (await _aggregate.ShouldTriggerNotificationAsync(rule, trigger, context, cancellationToken))
            {
                var content = new NotificationContent
                {
                    Subject = "Item Deleted",
                    Body = $"An item was deleted from your list by {notification.DeletedBy}",
                    ItemIdentifier = notification.Item.Id?.ToString(),
                    TriggeringUser = notification.DeletedBy,
                    OccurredOn = DateTime.UtcNow
                };

                await _aggregate.CreateNotificationAsync(
                    rule.UserId,
                    notification.Item.ListId.Value,
                    notification.Item.Id,
                    rule.Id,
                    content,
                    NotificationPriority.High,
                    cancellationToken);
            }
        }
    }
}