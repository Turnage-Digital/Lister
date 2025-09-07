using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers;

public class NotifyOnItemCreatedEventHandler : INotificationHandler<ListItemCreatedEvent>
{
    private readonly NotificationAggregate<NotificationRuleDb, NotificationDb> _aggregate;

    public NotifyOnItemCreatedEventHandler(NotificationAggregate<NotificationRuleDb, NotificationDb> aggregate)
    {
        _aggregate = aggregate;
    }

    public async Task Handle(ListItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Item.ListId == null) return;

        // Get all active rules for this list that trigger on item creation
        var rules = await _aggregate.GetActiveRulesForListAsync(
            notification.Item.ListId.Value,
            TriggerType.ItemCreated,
            cancellationToken);

        foreach (var rule in rules)
        {
            var trigger = NotificationTrigger.ItemCreated();
            var context = new Dictionary<string, object>
            {
                ["ItemId"] = notification.Item.Id!,
                ["CreatedBy"] = notification.CreatedBy
            };

            if (await _aggregate.ShouldTriggerNotificationAsync(rule, trigger, context, cancellationToken))
            {
                var content = new NotificationContent
                {
                    Subject = "New Item Created",
                    Body = $"A new item was created in your list by {notification.CreatedBy}",
                    ItemIdentifier = notification.Item.Id?.ToString(),
                    TriggeringUser = notification.CreatedBy,
                    OccurredOn = DateTime.UtcNow
                };

                await _aggregate.CreateNotificationAsync(
                    rule.UserId,
                    notification.Item.ListId.Value,
                    notification.Item.Id,
                    rule.Id,
                    content,
                    NotificationPriority.Normal,
                    cancellationToken);
            }
        }
    }
}