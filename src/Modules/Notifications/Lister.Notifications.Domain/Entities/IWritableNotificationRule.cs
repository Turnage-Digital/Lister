namespace Lister.Notifications.Domain.Entities;

public interface IWritableNotificationRule : INotificationRule
{
    new Guid? Id { get; set; }
    new string UserId { get; set; }
    new Guid ListId { get; set; }
    new bool IsActive { get; set; }
}