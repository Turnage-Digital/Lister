namespace Lister.Core;

public interface IGetListById<TList>
    where TList : IReadOnlyList
{
    Task<TList> GetAsync(string userId, Guid id, CancellationToken cancellationToken = default);
}