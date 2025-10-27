using Lister.Core.Domain.IntegrationEvents;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.ReadOnly.Dtos;
using Lister.Notifications.ReadOnly.Queries;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers.ListItemCreated;

public class NotifyEventHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate,
    IGetActiveNotificationRules queryService
) : INotificationHandler<ListItemCreatedIntegrationEvent>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    public async Task Handle(ListItemCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var rules = await queryService.GetAsync(
            notification.ListId,
            new[]
            {
                TriggerType.ItemCreated
            },
            cancellationToken);

        foreach (var ruleDto in rules)
        {
            var trigger = NotificationTrigger.ItemCreated();
            var context = new Dictionary<string, object>
            {
                ["ItemId"] = notification.ItemId ?? 0,
                ["CreatedBy"] = notification.CreatedBy
            };

            if (ruleDto.Id is null)
            {
                continue;
            }

            var rule = await aggregate.GetNotificationRuleByIdAsync(ruleDto.Id.Value, cancellationToken);
            if (rule is null)
            {
                continue;
            }

            if (rule is TNotificationRule typedRule &&
                await aggregate.ShouldTriggerNotificationAsync(typedRule, trigger, context, cancellationToken))
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
