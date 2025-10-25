namespace Lister.Lists.Domain.Queries;

public interface IGetListItemCount
{
    Task<int> CountAsync(Guid listId, CancellationToken cancellationToken);
}