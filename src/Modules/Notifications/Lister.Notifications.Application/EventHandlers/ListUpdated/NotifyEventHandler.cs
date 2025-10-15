using Lister.Core.Domain.IntegrationEvents;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Services;
using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers.ListUpdated;

public class NotifyEventHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate,
    IGetActiveNotificationRules queryService
) : INotificationHandler<ListUpdatedIntegrationEvent>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    public async Task Handle(ListUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var rules = await queryService.GetAsync(
            notification.ListId,
            TriggerType.ListUpdated,
            cancellationToken);

        foreach (var rule in rules)
        {
            var trigger = NotificationTrigger.ListUpdated();
            var context = new Dictionary<string, object>
            {
                ["ListId"] = notification.ListId,
                ["ListName"] = notification.ListName,
                ["UpdatedBy"] = notification.UpdatedBy
            };

            if (rule is TNotificationRule r &&
                await aggregate.ShouldTriggerNotificationAsync(r, trigger, context, cancellationToken))
            {
                var content = new NotificationContent
                {
                    Subject = "List Updated",
                    Body = $"The list '{notification.ListName}' was updated by {notification.UpdatedBy}",
                    ListName = notification.ListName,
                    TriggeringUser = notification.UpdatedBy,
                    OccurredOn = DateTime.UtcNow
                };

                await aggregate.CreateNotificationAsync(
                    r.UserId,
                    notification.ListId,
                    null,
                    r.Id,
                    content,
                    NotificationPriority.Normal,
                    cancellationToken);
            }
        }
    }
}