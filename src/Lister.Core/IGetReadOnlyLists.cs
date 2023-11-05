namespace Lister.Core;

public interface IGetReadOnlyLists<TList>
    where TList : IReadOnlyList
{
    Task<TList[]> GetAsync(string userId, CancellationToken cancellationToken = default);
}