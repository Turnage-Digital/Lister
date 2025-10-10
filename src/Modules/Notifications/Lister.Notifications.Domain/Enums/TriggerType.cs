namespace Lister.Notifications.Domain.Enums;

public enum TriggerType
{
    ItemCreated,
    ItemDeleted,
    ItemUpdated,
    StatusChanged,
    ColumnValueChanged,
    ListDeleted,
    ListUpdated,
    CustomCondition
}