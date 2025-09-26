namespace Lister.Notifications.Domain;

public interface INotification
{
    Guid? Id { get; }
    Guid? NotificationRuleId { get; }
    string UserId { get; }
    Guid ListId { get; }
    int? ItemId { get; }
}