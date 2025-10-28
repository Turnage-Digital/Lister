namespace Lister.Notifications.Domain.Entities;

public interface IWritableNotification
{
    Guid? Id { get; }
    Guid? NotificationRuleId { get; }
    string UserId { get; }
    Guid ListId { get; }
    int? ItemId { get; }
}