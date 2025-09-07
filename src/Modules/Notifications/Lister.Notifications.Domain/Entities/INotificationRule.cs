namespace Lister.Notifications.Domain.Entities;

public interface INotificationRule
{
    Guid? Id { get; }
    string UserId { get; }
    Guid ListId { get; }
    bool IsActive { get; }
}