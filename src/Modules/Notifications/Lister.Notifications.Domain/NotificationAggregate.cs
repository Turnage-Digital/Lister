using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Events;
using Lister.Notifications.Domain.ValueObjects;
using MediatR;

namespace Lister.Notifications.Domain;

public class NotificationAggregate<TRule, TNotification>(
    INotificationsUnitOfWork<TRule, TNotification> unitOfWork,
    IMediator mediator
)
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
    // public async Task<TList?> GetListByIdAsync(Guid id, CancellationToken cancellationToken = default)
    // {
    //     var retval = await unitOfWork.ListsStore.GetByIdAsync(id, cancellationToken);
    //     return retval;
    // }
    
    public async Task<TRule?> GetNotificationRuleByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await unitOfWork.RulesStore.GetByIdAsync(id, cancellationToken);
        return retval;
    }
    
    public async Task<TRule> CreateNotificationRuleAsync(
        string userId,
        string listId,
        NotificationTrigger trigger,
        NotificationChannel[] channels,
        NotificationSchedule schedule,
        string? templateId = null,
        CancellationToken cancellationToken = default
    )
    {
        var parsedListId = Guid.Parse(listId);
        var retval = await unitOfWork.RulesStore.InitAsync(userId, parsedListId, cancellationToken);
        await unitOfWork.RulesStore.SetTriggerAsync(retval, trigger, cancellationToken);
        await unitOfWork.RulesStore.SetChannelsAsync(retval, channels, cancellationToken);
        await unitOfWork.RulesStore.SetScheduleAsync(retval, schedule, cancellationToken);

        if (!string.IsNullOrEmpty(templateId))
        {
            await unitOfWork.RulesStore.SetTemplateAsync(retval, templateId, cancellationToken);
        }

        await unitOfWork.RulesStore.CreateAsync(retval, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new NotificationRuleCreatedEvent(retval, userId), cancellationToken);

        return retval;
    }

    public async Task UpdateNotificationRuleAsync(
        TRule rule,
        string updatedBy,
        NotificationTrigger? trigger = null,
        NotificationChannel[]? channels = null,
        NotificationSchedule? schedule = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default
    )
    {
        if (trigger != null)
        {
            await unitOfWork.RulesStore.SetTriggerAsync(rule, trigger, cancellationToken);
        }

        if (channels != null)
        {
            await unitOfWork.RulesStore.SetChannelsAsync(rule, channels, cancellationToken);
        }

        if (schedule != null)
        {
            await unitOfWork.RulesStore.SetScheduleAsync(rule, schedule, cancellationToken);
        }

        if (isActive.HasValue)
        {
            await unitOfWork.RulesStore.SetActiveStatusAsync(rule, isActive.Value, cancellationToken);
        }

        await unitOfWork.RulesStore.UpdateAsync(rule, updatedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new NotificationRuleUpdatedEvent(rule, updatedBy), cancellationToken);
    }

    public async Task DeleteNotificationRuleAsync(
        TRule rule,
        string deletedBy,
        CancellationToken cancellationToken = default
    )
    {
        await unitOfWork.RulesStore.DeleteAsync(rule, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new NotificationRuleDeletedEvent(rule, deletedBy), cancellationToken);
    }

    public async Task<TNotification> CreateNotificationAsync(
        string userId,
        Guid listId,
        int? itemId,
        Guid? ruleId,
        NotificationContent content,
        NotificationPriority priority = NotificationPriority.Normal,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await unitOfWork.NotificationsStore.InitAsync(
            userId, listId, cancellationToken);

        await unitOfWork.NotificationsStore.SetContentAsync(
            retval, content, cancellationToken);

        await unitOfWork.NotificationsStore.SetPriorityAsync(
            retval, priority, cancellationToken);

        if (itemId.HasValue)
        {
            await unitOfWork.NotificationsStore.SetItemAsync(
                retval, itemId.Value, cancellationToken);
        }

        if (ruleId.HasValue)
        {
            await unitOfWork.NotificationsStore.SetRuleAsync(
                retval, ruleId.Value, cancellationToken);
        }

        await unitOfWork.NotificationsStore.CreateAsync(retval, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new NotificationCreatedEvent(retval, userId), cancellationToken);

        return retval;
    }

    public async Task MarkNotificationAsProcessedAsync(
        TNotification notification,
        CancellationToken cancellationToken = default
    )
    {
        await unitOfWork.NotificationsStore.MarkAsProcessedAsync(
            notification, DateTime.UtcNow, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(
            new NotificationProcessedEvent(notification),
            cancellationToken);
    }

    public async Task RecordDeliveryAttemptAsync(
        TNotification notification,
        NotificationChannel channel,
        DeliveryStatus status,
        string? failureReason = null,
        CancellationToken cancellationToken = default
    )
    {
        var attempt = new NotificationDeliveryAttempt
        {
            Type = status,
            On = DateTime.UtcNow,
            By = "System",
            Channel = channel,
            FailureReason = failureReason,
            AttemptNumber = await unitOfWork.NotificationsStore
                .GetDeliveryAttemptCountAsync(notification, channel, cancellationToken) + 1
        };

        await unitOfWork.NotificationsStore.AddDeliveryAttemptAsync(
            notification, attempt, cancellationToken);

        if (status == DeliveryStatus.Delivered)
        {
            await unitOfWork.NotificationsStore.MarkAsDeliveredAsync(
                notification, DateTime.UtcNow, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new NotificationDeliveryAttemptedEvent(notification, channel, status),
            cancellationToken);
    }


    // Rule Evaluation
    public async Task<bool> ShouldTriggerNotificationAsync(
        TRule rule,
        NotificationTrigger actualTrigger,
        Dictionary<string, object> context,
        CancellationToken cancellationToken = default
    )
    {
        var ruleTrigger = await unitOfWork.RulesStore.GetTriggerAsync(
            rule, cancellationToken);

        // Basic type matching
        if (ruleTrigger.Type != actualTrigger.Type)
        {
            return false;
        }

        // Specific condition matching based on trigger type
        switch (ruleTrigger.Type)
        {
            case TriggerType.StatusChanged:
                if (!string.IsNullOrEmpty(ruleTrigger.FromValue) &&
                    ruleTrigger.FromValue != actualTrigger.FromValue)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(ruleTrigger.ToValue) &&
                    ruleTrigger.ToValue != actualTrigger.ToValue)
                {
                    return false;
                }

                break;

            case TriggerType.ColumnValueChanged:
                if (ruleTrigger.ColumnName != actualTrigger.ColumnName)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(ruleTrigger.FromValue) &&
                    ruleTrigger.FromValue != actualTrigger.FromValue)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(ruleTrigger.ToValue) &&
                    ruleTrigger.ToValue != actualTrigger.ToValue)
                {
                    return false;
                }

                break;
        }

        return true;
    }

    public async Task<IEnumerable<TNotification>> GetUserNotificationsAsync(
        string userId,
        DateTime? since = null,
        int pageSize = 20,
        int page = 0,
        CancellationToken cancellationToken = default
    )
    {
        return await unitOfWork.NotificationsStore.GetUserNotificationsAsync(
            userId, since, pageSize, page, cancellationToken);
    }

    public async Task<TNotification?> GetNotificationByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        return await unitOfWork.NotificationsStore.GetNotificationByIdAsync(
            id, userId, cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(
        string userId,
        Guid? listId = null,
        CancellationToken cancellationToken = default
    )
    {
        return await unitOfWork.NotificationsStore.GetUnreadCountAsync(
            userId, listId, cancellationToken);
    }

    public async Task MarkNotificationAsReadAsync(
        TNotification notification,
        DateTime readOn,
        CancellationToken cancellationToken = default
    )
    {
        await unitOfWork.NotificationsStore.MarkAsReadAsync(
            notification, readOn, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(
            new NotificationReadEvent(notification),
            cancellationToken);
    }

    public async Task MarkAllNotificationsAsReadAsync(
        string userId,
        DateTime readOn,
        DateTime? before = null,
        CancellationToken cancellationToken = default
    )
    {
        await unitOfWork.NotificationsStore.MarkAllAsReadAsync(
            userId, readOn, before, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(
            new AllNotificationsReadEvent(userId, readOn, before),
            cancellationToken);
    }
}