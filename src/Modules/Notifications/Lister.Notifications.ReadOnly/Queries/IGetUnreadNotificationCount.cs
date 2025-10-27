namespace Lister.Notifications.ReadOnly.Queries;

public interface IGetUnreadNotificationCount
{
    Task<int> GetAsync(string userId, Guid? listId, CancellationToken cancellationToken);
}
