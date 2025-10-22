using System.Text.Json.Serialization;
using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.Domain.ValueObjects;

public record NotificationTrigger
{
    [JsonPropertyName("type")]
    public TriggerType Type { get; init; }

    [JsonPropertyName("fromValue")]
    public string? FromValue { get; init; }

    [JsonPropertyName("toValue")]
    public string? ToValue { get; init; }

    [JsonPropertyName("columnName")]
    public string? ColumnName { get; init; }

    [JsonPropertyName("operator")]
    public string? Operator { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }

    public static NotificationTrigger ItemCreated()
    {
        return new NotificationTrigger { Type = TriggerType.ItemCreated };
    }

    public static NotificationTrigger ItemDeleted()
    {
        return new NotificationTrigger { Type = TriggerType.ItemDeleted };
    }

    public static NotificationTrigger ItemUpdated()
    {
        return new NotificationTrigger { Type = TriggerType.ItemUpdated };
    }

    public static NotificationTrigger StatusChanged(string from, string to)
    {
        return new NotificationTrigger
        {
            Type = TriggerType.StatusChanged,
            FromValue = from,
            ToValue = to
        };
    }

    public static NotificationTrigger AnyStatusChange()
    {
        return new NotificationTrigger { Type = TriggerType.StatusChanged };
    }

    public static NotificationTrigger ColumnValueChanged(string columnName, string from, string to)
    {
        return new NotificationTrigger
        {
            Type = TriggerType.ColumnValueChanged,
            ColumnName = columnName,
            FromValue = from,
            ToValue = to
        };
    }

    public static NotificationTrigger ListDeleted()
    {
        return new NotificationTrigger { Type = TriggerType.ListDeleted };
    }

    public static NotificationTrigger ListUpdated()
    {
        return new NotificationTrigger { Type = TriggerType.ListUpdated };
    }
}