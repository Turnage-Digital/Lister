namespace Lister.Notifications.Domain.Entities;

public interface IWritableNotificationRule
{
    Guid? Id { get; }
    string UserId { get; }
    Guid ListId { get; }
}