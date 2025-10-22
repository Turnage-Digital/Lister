using System.Globalization;
using System.Text.RegularExpressions;
using Lister.Notifications.Domain.Entities;
using Lister.Notifications.Domain.Enums;
using Lister.Notifications.Domain.ValueObjects;

namespace Lister.Notifications.Domain.Services;

public class NotificationTriggerEvaluator : INotificationTriggerEvaluator
{
    public async Task<bool> ShouldTriggerAsync<TNotificationRule>(
        INotificationRulesStore<TNotificationRule> rulesStore,
        TNotificationRule rule,
        NotificationTrigger actualTrigger,
        Dictionary<string, object> context,
        CancellationToken cancellationToken
    ) where TNotificationRule : IWritableNotificationRule
    {
        var ruleTrigger = await rulesStore.GetTriggerAsync(rule, cancellationToken);

        if (ruleTrigger.Type != actualTrigger.Type)
        {
            return false;
        }

        return ruleTrigger.Type switch
        {
            TriggerType.ItemCreated
                or TriggerType.ItemDeleted
                or TriggerType.ItemUpdated
                or TriggerType.ListDeleted
                or TriggerType.ListUpdated => true,

            TriggerType.StatusChanged => EvaluateStatusChanged(ruleTrigger, actualTrigger),

            TriggerType.ColumnValueChanged => EvaluateColumnValueChanged(ruleTrigger, actualTrigger),

            TriggerType.CustomCondition => EvaluateCustomCondition(ruleTrigger, context),

            _ => false
        };
    }

    private static bool EvaluateStatusChanged(NotificationTrigger ruleTrigger, NotificationTrigger actualTrigger)
    {
        if (!string.IsNullOrEmpty(ruleTrigger.FromValue) &&
            !string.Equals(ruleTrigger.FromValue, actualTrigger.FromValue, StringComparison.Ordinal))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(ruleTrigger.ToValue) &&
            !string.Equals(ruleTrigger.ToValue, actualTrigger.ToValue, StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private static bool EvaluateColumnValueChanged(NotificationTrigger ruleTrigger, NotificationTrigger actualTrigger)
    {
        if (!string.Equals(ruleTrigger.ColumnName, actualTrigger.ColumnName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(ruleTrigger.FromValue) &&
            !string.Equals(ruleTrigger.FromValue, actualTrigger.FromValue, StringComparison.Ordinal))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(ruleTrigger.ToValue) &&
            !string.Equals(ruleTrigger.ToValue, actualTrigger.ToValue, StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private static bool EvaluateCustomCondition(NotificationTrigger ruleTrigger, Dictionary<string, object> context)
    {
        if (string.IsNullOrEmpty(ruleTrigger.Operator) || string.IsNullOrEmpty(ruleTrigger.ColumnName))
        {
            return false;
        }

        if (!context.TryGetValue(ruleTrigger.ColumnName, out var actualValue))
        {
            return false;
        }

        return EvaluateCustomCondition(ruleTrigger.Operator, actualValue, ruleTrigger.Value);
    }

    private static bool EvaluateCustomCondition(string operatorType, object? actualValue, string? expectedValue)
    {
        if (operatorType.Equals("isnull", StringComparison.InvariantCultureIgnoreCase))
        {
            return actualValue is null;
        }

        if (operatorType.Equals("isnotnull", StringComparison.InvariantCultureIgnoreCase))
        {
            return actualValue is not null;
        }

        if (actualValue is null || expectedValue is null)
        {
            return false;
        }

        var actual = Convert.ToString(actualValue, CultureInfo.InvariantCulture) ?? string.Empty;
        var expected = expectedValue;

        return operatorType.ToLowerInvariant() switch
        {
            "=" or "==" or "equals" =>
                actual.Equals(expected, StringComparison.OrdinalIgnoreCase),

            "!=" or "<>" or "notequals" =>
                !actual.Equals(expected, StringComparison.OrdinalIgnoreCase),

            ">" or "greaterthan" =>
                CompareValues(actual, expected) > 0,

            "<" or "lessthan" =>
                CompareValues(actual, expected) < 0,

            ">=" or "greaterthanorequal" =>
                CompareValues(actual, expected) >= 0,

            "<=" or "lessthanorequal" =>
                CompareValues(actual, expected) <= 0,

            "contains" =>
                actual.Contains(expected, StringComparison.OrdinalIgnoreCase),

            "startswith" =>
                actual.StartsWith(expected, StringComparison.OrdinalIgnoreCase),

            "endswith" =>
                actual.EndsWith(expected, StringComparison.OrdinalIgnoreCase),

            "in" =>
                expected.Split(',').Any(v => v.Trim().Equals(actual, StringComparison.OrdinalIgnoreCase)),

            "notin" =>
                !expected.Split(',').Any(v => v.Trim().Equals(actual, StringComparison.OrdinalIgnoreCase)),

            "regex" =>
                Regex.IsMatch(actual, expected),

            _ => false
        };
    }

    private static int CompareValues(string actualValue, string expectedValue)
    {
        if (decimal.TryParse(actualValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var actualNumber) &&
            decimal.TryParse(expectedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var expectedNumber))
        {
            return actualNumber.CompareTo(expectedNumber);
        }

        if (DateTime.TryParse(actualValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var actualDate) &&
            DateTime.TryParse(expectedValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var expectedDate))
        {
            return actualDate.CompareTo(expectedDate);
        }

        return string.Compare(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);
    }
}