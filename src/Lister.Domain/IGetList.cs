namespace Lister.Domain;

public interface IGetList<TList>
    where TList : IList
{
    Task<TList?> Get(string userId, Guid listId, CancellationToken cancellationToken);
}