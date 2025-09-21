// File: src/Modules/Notifications/Lister.Notifications.Domain/Enums/TriggerType.cs

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