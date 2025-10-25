using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Queries;

public interface IGetListMigrationJobs
{
    Task<ListMigrationJob[]> GetAsync(Guid listId, CancellationToken cancellationToken);
}