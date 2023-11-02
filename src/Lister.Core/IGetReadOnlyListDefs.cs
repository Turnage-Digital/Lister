namespace Lister.Core;

public interface IGetReadOnlyListDefs<TListDef>
    where TListDef : IReadOnlyListDef
{
    Task<TListDef[]> GetAsync(string userId, CancellationToken cancellationToken = default);
}