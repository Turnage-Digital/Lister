namespace Lister.Core;

public interface IGetLists<TList>
    where TList : IReadOnlyList
{
    Task<TList[]> GetAsync(string userId, CancellationToken cancellationToken = default);
}