namespace Lister.Notifications.Domain;

public interface INotificationRule
{
    Guid? Id { get; }
    string UserId { get; }
    Guid ListId { get; }
}