using Lister.Core.Domain.IntegrationEvents;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers.ListItemUpdated;

public class NotifyEventHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate,
    IGetActiveNotificationRules queryService
) : INotificationHandler<ListItemUpdatedIntegrationEvent>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    public async Task Handle(ListItemUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var rules = await queryService.GetAsync(
            notification.ListId,
            TriggerType.ItemUpdated,
            cancellationToken);

        foreach (var rule in rules)
        {
            var trigger = NotificationTrigger.ItemUpdated();
            var context = new Dictionary<string, object>
            {
                ["ItemId"] = notification.ItemId ?? 0,
                ["UpdatedBy"] = notification.UpdatedBy,
                ["PreviousBag"] = notification.PreviousBag ?? new Dictionary<string, object?>(),
                ["NewBag"] = notification.NewBag
            };

            if (rule is TNotificationRule r &&
                await aggregate.ShouldTriggerNotificationAsync(r, trigger, context, cancellationToken))
            {
                var content = new NotificationContent
                {
                    Subject = "Item Updated",
                    Body = $"An item was updated in your list by {notification.UpdatedBy}",
                    ItemIdentifier = notification.ItemId?.ToString(),
                    TriggeringUser = notification.UpdatedBy,
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