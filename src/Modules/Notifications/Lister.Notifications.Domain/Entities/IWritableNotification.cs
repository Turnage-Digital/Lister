namespace Lister.Notifications.Domain.Entities;

public interface IWritableNotification : INotification
{
    new Guid? Id { get; set; }
    new Guid? NotificationRuleId { get; set; }
    new string UserId { get; set; }
    new Guid ListId { get; set; }
    new int? ItemId { get; set; }
    new DateTime CreatedOn { get; set; }
    new DateTime? ProcessedOn { get; set; }
    new DateTime? DeliveredOn { get; set; }
    new DateTime? ReadOn { get; set; }
}