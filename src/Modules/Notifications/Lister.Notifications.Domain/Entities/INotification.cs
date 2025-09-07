namespace Lister.Notifications.Domain.Entities;

public interface INotification
{
    Guid? Id { get; }
    Guid? NotificationRuleId { get; }
    string UserId { get; }
    Guid ListId { get; }
    int? ItemId { get; }
    DateTime CreatedOn { get; }
    DateTime? ProcessedOn { get; }
    DateTime? DeliveredOn { get; }
}