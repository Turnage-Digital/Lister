using System.Text.RegularExpressions;
using Lister.Core.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.Events;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Domain;

public class NotificationAggregate<TRule, TNotification>(
    INotificationsUnitOfWork<TRule, TNotification> unitOfWork,
    IDomainEventQueue events
)
    where TRule : IWritableNotificationRule
    where TNotification : IWritableNotification
{
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
        Guid listId,
        NotificationTrigger trigger,
        NotificationChannel[] channels,
        NotificationSchedule schedule,
        string? templateId = null,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await unitOfWork.RulesStore.InitAsync(userId, listId, cancellationToken);
        await unitOfWork.RulesStore.SetTriggerAsync(retval, trigger, cancellationToken);
        await unitOfWork.RulesStore.SetChannelsAsync(retval, channels, cancellationToken);
        await unitOfWork.RulesStore.SetScheduleAsync(retval, schedule, cancellationToken);

        if (!string.IsNullOrEmpty(templateId))
        {
            await unitOfWork.RulesStore.SetTemplateAsync(retval, templateId, cancellationToken);
        }

        await unitOfWork.RulesStore.CreateAsync(retval, cancellationToken);
        events.Enqueue(new NotificationRuleCreatedEvent(retval, userId), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
        if (trigger is not null)
        {
            await unitOfWork.RulesStore.SetTriggerAsync(rule, trigger, cancellationToken);
        }

        if (channels is not null)
        {
            await unitOfWork.RulesStore.SetChannelsAsync(rule, channels, cancellationToken);
        }

        if (schedule is not null)
        {
            await unitOfWork.RulesStore.SetScheduleAsync(rule, schedule, cancellationToken);
        }

        if (isActive is not null)
        {
            await unitOfWork.RulesStore.SetActiveStatusAsync(rule, isActive.Value, cancellationToken);
        }

        await unitOfWork.RulesStore.UpdateAsync(rule, updatedBy, cancellationToken);
        events.Enqueue(new NotificationRuleUpdatedEvent(rule, updatedBy), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteNotificationRuleAsync(
        TRule rule,
        string deletedBy,
        CancellationToken cancellationToken = default
    )
    {
        await unitOfWork.RulesStore.DeleteAsync(rule, deletedBy, cancellationToken);
        events.Enqueue(new NotificationRuleDeletedEvent(rule, deletedBy), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<TNotification?> GetNotificationByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await unitOfWork.NotificationsStore.GetByIdAsync(id, userId, cancellationToken);
        return retval;
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

        if (itemId is not null)
        {
            await unitOfWork.NotificationsStore.SetItemAsync(
                retval, itemId.Value, cancellationToken);
        }

        if (ruleId is not null)
        {
            await unitOfWork.NotificationsStore.SetRuleAsync(
                retval, ruleId.Value, cancellationToken);
        }

        await unitOfWork.NotificationsStore.CreateAsync(retval, cancellationToken);
        events.Enqueue(new NotificationCreatedEvent(retval, userId), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return retval;
    }

    public async Task MarkNotificationAsProcessedAsync(
        TNotification notification,
        CancellationToken cancellationToken = default
    )
    {
        await unitOfWork.NotificationsStore.MarkAsProcessedAsync(
            notification, DateTime.UtcNow, cancellationToken);
        events.Enqueue(new NotificationProcessedEvent(notification), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
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

        events.Enqueue(new NotificationDeliveryAttemptedEvent(notification, channel, status), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkNotificationAsReadAsync(
        TNotification notification,
        DateTime readOn,
        CancellationToken cancellationToken = default
    )
    {
        await unitOfWork.NotificationsStore.MarkAsReadAsync(
            notification, readOn, cancellationToken);
        events.Enqueue(new NotificationReadEvent(notification), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
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
        events.Enqueue(new AllNotificationsReadEvent(userId, readOn, before), EventPhase.AfterSave);
        await unitOfWork.SaveChangesAsync(cancellationToken);
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
            case TriggerType.ItemCreated:
            case TriggerType.ItemDeleted:
            case TriggerType.ItemUpdated:
            case TriggerType.ListDeleted:
            case TriggerType.ListUpdated:
                // These are simple triggers with no additional conditions
                // The type match is enough
                return true;

            case TriggerType.StatusChanged:
                // If rule specifies a "from" status, it must match
                if (!string.IsNullOrEmpty(ruleTrigger.FromValue) &&
                    ruleTrigger.FromValue != actualTrigger.FromValue)
                {
                    return false;
                }

                // If rule specifies a "to" status, it must match
                if (!string.IsNullOrEmpty(ruleTrigger.ToValue) &&
                    ruleTrigger.ToValue != actualTrigger.ToValue)
                {
                    return false;
                }

                // If no specific statuses are specified, any status change triggers
                return true;

            case TriggerType.ColumnValueChanged:
                // Column name must match
                if (ruleTrigger.ColumnName != actualTrigger.ColumnName)
                {
                    return false;
                }

                // If rule specifies a "from" value, it must match
                if (!string.IsNullOrEmpty(ruleTrigger.FromValue) &&
                    ruleTrigger.FromValue != actualTrigger.FromValue)
                {
                    return false;
                }

                // If rule specifies a "to" value, it must match
                if (!string.IsNullOrEmpty(ruleTrigger.ToValue) &&
                    ruleTrigger.ToValue != actualTrigger.ToValue)
                {
                    return false;
                }

                return true;

            case TriggerType.CustomCondition:
                // Custom conditions use the operator and value fields with context
                if (string.IsNullOrEmpty(ruleTrigger.Operator) ||
                    string.IsNullOrEmpty(ruleTrigger.ColumnName))
                {
                    return false;
                }

                // Get the actual value from context
                if (!context.TryGetValue(ruleTrigger.ColumnName, out var actualValue))
                {
                    // If the field doesn't exist in context, condition fails
                    return false;
                }

                // Evaluate the condition based on operator (allow null actuals)
                return EvaluateCustomCondition(
                    ruleTrigger.Operator,
                    actualValue?.ToString(),
                    ruleTrigger.Value);

            default:
                return false;
        }
    }

    private static bool EvaluateCustomCondition(string operatorType, string? actualValue, string? expectedValue)
    {
        // Handle null cases
        if (operatorType.Equals("isnull", StringComparison.InvariantCultureIgnoreCase))
        {
            return actualValue == null;
        }

        if (operatorType.Equals("isnotnull", StringComparison.InvariantCultureIgnoreCase))
        {
            return actualValue is not null;
        }

        // For other operators, both values must be non-null
        if (actualValue == null || expectedValue == null)
        {
            return false;
        }

        return operatorType.ToLowerInvariant() switch
        {
            "=" or "==" or "equals" =>
                actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase),

            "!=" or "<>" or "notequals" =>
                !actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase),

            ">" or "greaterthan" =>
                CompareValues(actualValue, expectedValue) > 0,

            "<" or "lessthan" =>
                CompareValues(actualValue, expectedValue) < 0,

            ">=" or "greaterthanorequal" =>
                CompareValues(actualValue, expectedValue) >= 0,

            "<=" or "lessthanorequal" =>
                CompareValues(actualValue, expectedValue) <= 0,

            "contains" =>
                actualValue.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),

            "startswith" =>
                actualValue.StartsWith(expectedValue, StringComparison.OrdinalIgnoreCase),

            "endswith" =>
                actualValue.EndsWith(expectedValue, StringComparison.OrdinalIgnoreCase),

            "in" =>
                expectedValue.Split(',').Any(v => v.Trim().Equals(actualValue, StringComparison.OrdinalIgnoreCase)),

            "notin" =>
                !expectedValue.Split(',').Any(v => v.Trim().Equals(actualValue, StringComparison.OrdinalIgnoreCase)),

            "regex" =>
                Regex.IsMatch(actualValue, expectedValue),

            _ => false
        };
    }

    private static int CompareValues(string actualValue, string expectedValue)
    {
        // Try to compare as numbers first
        if (decimal.TryParse(actualValue, out var actualNumber) &&
            decimal.TryParse(expectedValue, out var expectedNumber))
        {
            return actualNumber.CompareTo(expectedNumber);
        }

        // Try to compare as dates
        if (DateTime.TryParse(actualValue, out var actualDate) &&
            DateTime.TryParse(expectedValue, out var expectedDate))
        {
            return actualDate.CompareTo(expectedDate);
        }

        // Fall back to string comparison
        return string.Compare(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);
    }
}