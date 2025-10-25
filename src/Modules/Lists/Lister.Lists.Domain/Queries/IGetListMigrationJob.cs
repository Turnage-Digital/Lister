using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Queries;

public interface IGetListMigrationJob
{
    Task<ListMigrationJob?> GetAsync(Guid listId, Guid jobId, CancellationToken cancellationToken);
}