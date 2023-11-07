namespace Lister.Core;

public interface IGetListNames<T>
{
    Task<T[]> GetAsync(string userId, CancellationToken cancellationToken = default);
}