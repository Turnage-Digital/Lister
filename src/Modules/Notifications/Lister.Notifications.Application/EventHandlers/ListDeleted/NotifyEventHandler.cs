using Lister.Core.Domain.IntegrationEvents;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.ReadOnly.Queries;
using MediatR;

namespace Lister.Notifications.Application.EventHandlers.ListDeleted;

public class NotifyEventHandler<TNotificationRule, TNotification>(
    NotificationAggregate<TNotificationRule, TNotification> aggregate,
    IGetActiveNotificationRules queryService
) : INotificationHandler<ListDeletedIntegrationEvent>
    where TNotificationRule : class, IWritableNotificationRule
    where TNotification : class, IWritableNotification
{
    public async Task Handle(ListDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var rules = await queryService.GetAsync(
            notification.ListId,
            new[]
            {
                TriggerType.ListDeleted
            },
            cancellationToken);

        foreach (var rule in rules)
        {
            var trigger = NotificationTrigger.ListDeleted();
            var context = new Dictionary<string, object>
            {
                ["ListId"] = notification.ListId,
                ["DeletedBy"] = notification.DeletedBy
            };

            if (rule is TNotificationRule r &&
                await aggregate.ShouldTriggerNotificationAsync(r, trigger, context, cancellationToken))
            {
                var content = new NotificationContent
                {
                    Subject = "List Deleted",
                    Body = $"The list '{notification.ListName}' was deleted by {notification.DeletedBy}",
                    ListName = notification.ListName,
                    TriggeringUser = notification.DeletedBy,
                    OccurredOn = DateTime.UtcNow
                };

                await aggregate.CreateNotificationAsync(
                    r.UserId,
                    notification.ListId,
                    null,
                    r.Id,
                    content,
                    NotificationPriority.Critical,
                    cancellationToken);
            }
        }
    }
}