using System.Collections;
using System.Globalization;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.ReadOnly.Dtos;
using Lister.Notifications.ReadOnly.Queries;
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
        var targetTriggers = new[]
        {
            TriggerType.ItemUpdated,
            TriggerType.StatusChanged,
            TriggerType.ColumnValueChanged,
            TriggerType.CustomCondition
        };

        var ruleDtos = (await queryService.GetAsync(
            notification.ListId,
            targetTriggers,
            cancellationToken)).ToArray();

        if (ruleDtos.Length == 0)
        {
            return;
        }

        var previousBag = ToDictionary(notification.PreviousBag);
        var newBag = ToDictionary(notification.NewBag);
        var context = BuildContext(notification, previousBag, newBag);
        var actualTriggers = BuildActualTriggers(previousBag, newBag);

        foreach (var ruleDto in ruleDtos)
        {
            if (ruleDto.Id is null)
            {
                continue;
            }

            var rule = await aggregate.GetNotificationRuleByIdAsync(ruleDto.Id.Value, cancellationToken);
            if (rule is not TNotificationRule r)
            {
                continue;
            }

            foreach (var trigger in actualTriggers)
            {
                if (!await aggregate.ShouldTriggerNotificationAsync(r, trigger, context, cancellationToken))
                {
                    continue;
                }

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

                break;
            }
        }
    }

    private static Dictionary<string, object?> ToDictionary(object? bag)
    {
        if (bag is null)
        {
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        }

        if (bag is IDictionary<string, object?> typed)
        {
            return new Dictionary<string, object?>(typed, StringComparer.OrdinalIgnoreCase);
        }

        if (bag is IDictionary dictionary)
        {
            var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry entry in dictionary)
            {
                if (entry.Key is string key)
                {
                    result[key] = entry.Value;
                }
            }

            return result;
        }

        return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, object> BuildContext(
        ListItemUpdatedIntegrationEvent notification,
        Dictionary<string, object?> previousBag,
        Dictionary<string, object?> newBag
    )
    {
        var context = new Dictionary<string, object>
        {
            ["ItemId"] = notification.ItemId ?? 0,
            ["UpdatedBy"] = notification.UpdatedBy,
            ["NewBag"] = newBag
        };

        if (previousBag.Count > 0)
        {
            context["PreviousBag"] = previousBag;
        }

        foreach (var kvp in newBag)
        {
            context[kvp.Key] = kvp.Value!;
        }

        foreach (var kvp in previousBag)
        {
            context[$"previous.{kvp.Key}"] = kvp.Value!;
        }

        return context;
    }

    private static IReadOnlyCollection<NotificationTrigger> BuildActualTriggers(
        Dictionary<string, object?> previousBag,
        Dictionary<string, object?> newBag
    )
    {
        var triggers = new List<NotificationTrigger>
        {
            NotificationTrigger.ItemUpdated()
        };

        var previousStatus = GetString(previousBag, "status");
        var newStatus = GetString(newBag, "status");

        if (!string.Equals(previousStatus, newStatus, StringComparison.OrdinalIgnoreCase))
        {
            triggers.Add(new NotificationTrigger
            {
                Type = TriggerType.StatusChanged,
                FromValue = previousStatus,
                ToValue = newStatus
            });
        }

        foreach (var change in GetColumnChanges(previousBag, newBag))
        {
            triggers.Add(new NotificationTrigger
            {
                Type = TriggerType.ColumnValueChanged,
                ColumnName = change.Column,
                FromValue = change.Previous,
                ToValue = change.Current
            });
        }

        if (newBag.Count > 0)
        {
            triggers.Add(new NotificationTrigger { Type = TriggerType.CustomCondition });
        }

        return triggers;
    }

    private static IEnumerable<(string Column, string? Previous, string? Current)> GetColumnChanges(
        Dictionary<string, object?> previousBag,
        Dictionary<string, object?> newBag
    )
    {
        var keys = new HashSet<string>(previousBag.Keys, StringComparer.OrdinalIgnoreCase);
        keys.UnionWith(newBag.Keys);

        foreach (var key in keys)
        {
            if (string.Equals(key, "status", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var previous = previousBag.TryGetValue(key, out var prevValue) ? prevValue : null;
            var current = newBag.TryGetValue(key, out var newValue) ? newValue : null;

            if (ValuesEqual(previous, current))
            {
                continue;
            }

            yield return (key, ConvertToString(previous), ConvertToString(current));
        }
    }

    private static string? GetString(Dictionary<string, object?> bag, string key)
    {
        if (!bag.TryGetValue(key, out var value) || value is null)
        {
            return null;
        }

        return ConvertToString(value);
    }

    private static bool ValuesEqual(object? left, object? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        if (left is IFormattable leftFormattable && right is IFormattable rightFormattable)
        {
            var leftStr = leftFormattable.ToString(null, CultureInfo.InvariantCulture);
            var rightStr = rightFormattable.ToString(null, CultureInfo.InvariantCulture);
            return string.Equals(leftStr, rightStr, StringComparison.OrdinalIgnoreCase);
        }

        return left.Equals(right);
    }

    private static string? ConvertToString(object? value)
    {
        return value switch
        {
            null => null,
            string str => str,
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }
}
