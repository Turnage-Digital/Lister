using Lister.Notifications.Domain.Enums;

namespace Lister.Notifications.Domain.ValueObjects;

public record NotificationTrigger
{
    public TriggerType Type { get; init; }

    public string? FromValue { get; init; }

    public string? ToValue { get; init; }

    public string? ColumnName { get; init; }

    public string? Operator { get; init; }

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