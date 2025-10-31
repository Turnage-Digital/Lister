using Lister.Lists.ReadOnly.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class MigrationJobStatusGetter(ListsDbContext dbContext) : IGetMigrationJobStatus
{
    public async Task<MigrationJobStatusDto?> GetAsync(
        Guid listId,
        Guid correlationId,
        CancellationToken cancellationToken
    )
    {
        var job = await dbContext.ListMigrationJobs
            .AsNoTracking()
            .Where(j => j.SourceListId == listId && j.CorrelationId == correlationId)
            .Select(j => new MigrationJobStatusDto(
                j.Id,
                j.SourceListId,
                j.CorrelationId,
                j.Stage,
                j.RequestedBy,
                j.CreatedOn,
                j.StartedOn,
                j.CompletedOn,
                j.BackupListId,
                j.NewListId,
                j.BackupExpiresOn,
                j.BackupRemovedOn,
                j.Attempts,
                j.LastError))
            .FirstOrDefaultAsync(cancellationToken);

        return job;
    }
}