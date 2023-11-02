namespace Lister.Core;

public interface IGetReadOnlyListDefById<TListDef>
    where TListDef : IReadOnlyListDef
{
    Task<TListDef> GetByIdAsync(string userId, Guid id, CancellationToken cancellationToken = default);
}