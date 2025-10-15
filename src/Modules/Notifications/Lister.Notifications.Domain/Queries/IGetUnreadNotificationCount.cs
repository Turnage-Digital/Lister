namespace Lister.Notifications.Domain.Queries;

public interface IGetUnreadNotificationCount
{
    Task<int> GetAsync(string userId, Guid? listId, CancellationToken cancellationToken);
}