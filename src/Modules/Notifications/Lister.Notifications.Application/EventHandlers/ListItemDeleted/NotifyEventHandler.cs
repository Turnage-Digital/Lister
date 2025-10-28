using Lister.Core.Domain.IntegrationEvents;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.ReadOnly.Queries;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers.ListItemDeleted;

public class NotifyEventHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate,
    IGetActiveNotificationRules queryService
) : INotificationHandler<ListItemDeletedIntegrationEvent>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    public async Task Handle(ListItemDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var rules = await queryService.GetAsync(
            notification.ListId,
            new[]
            {
                TriggerType.ItemDeleted
            },
            cancellationToken);

        foreach (var ruleDto in rules)
        {
            var trigger = NotificationTrigger.ItemDeleted();
            var context = new Dictionary<string, object>
            {
                ["ItemId"] = notification.ItemId ?? 0,
                ["DeletedBy"] = notification.DeletedBy
            };

            if (ruleDto.Id is null)
            {
                continue;
            }

            var rule = await aggregate.GetNotificationRuleByIdAsync(ruleDto.Id.Value, cancellationToken);
            if (rule is not TNotificationRule r)
            {
                continue;
            }

            if (await aggregate.ShouldTriggerNotificationAsync(r, trigger, context, cancellationToken))
            {
                var content = new NotificationContent
                {
                    Subject = "Item Deleted",
                    Body = $"An item was deleted from your list by {notification.DeletedBy}",
                    ItemIdentifier = notification.ItemId?.ToString(),
                    TriggeringUser = notification.DeletedBy,
                    OccurredOn = DateTime.UtcNow
                };

                await aggregate.CreateNotificationAsync(
                    rule.UserId,
                    notification.ListId,
                    notification.ItemId,
                    rule.Id,
                    content,
                    NotificationPriority.High,
                    cancellationToken);
            }
        }
    }
}