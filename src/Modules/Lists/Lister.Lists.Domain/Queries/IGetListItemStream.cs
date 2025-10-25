namespace Lister.Lists.Domain.Queries;

public interface IGetListItemStream
{
    IAsyncEnumerable<int> StreamAsync(Guid listId, CancellationToken cancellationToken);
}