namespace Lister.Core;

public interface IGetListNames<TList>
    where TList : IReadOnlyList
{
    Task<TList[]> GetAsync(string userId, CancellationToken cancellationToken = default);
}