namespace Lister.Notifications.Domain.Services;

public interface IGetUnreadNotificationCount
{
    Task<int> GetAsync(string userId, Guid? listId, CancellationToken cancellationToken);
}