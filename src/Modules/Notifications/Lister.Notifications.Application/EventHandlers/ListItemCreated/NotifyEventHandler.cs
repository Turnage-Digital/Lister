using Lister.Core.Domain.IntegrationEvents;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Infrastructure.Sql.Services;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers.ListItemCreated;

public class NotifyEventHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate,
    INotificationRuleQueryService queryService
)
    : INotificationHandler<ListItemCreatedIntegrationEvent>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    public async Task Handle(ListItemCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        // Get all active rules for this list that trigger on item creation
        var rules = await queryService.GetActiveRulesForListAsync(
            notification.ListId,
            TriggerType.ItemCreated,
            cancellationToken);

        foreach (var rule in rules)
        {
            var trigger = NotificationTrigger.ItemCreated();
            var context = new Dictionary<string, object>
            {
                ["ItemId"] = notification.ItemId ?? 0,
                ["CreatedBy"] = notification.CreatedBy
            };

            if (await aggregate.ShouldTriggerNotificationAsync(rule as TNotificationRule, trigger, context,
                    cancellationToken))
            {
                var content = new NotificationContent
                {
                    Subject = "New Item Created",
                    Body = $"A new item was created in your list by {notification.CreatedBy}",
                    ItemIdentifier = notification.ItemId?.ToString(),
                    TriggeringUser = notification.CreatedBy,
                    OccurredOn = DateTime.UtcNow
                };

                await aggregate.CreateNotificationAsync(
                    rule.UserId,
                    notification.ListId,
                    notification.ItemId,
                    rule.Id,
                    content,
                    NotificationPriority.Normal,
                    cancellationToken);
            }
        }
    }
}