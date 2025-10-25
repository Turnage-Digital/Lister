using System.Linq.Expressions;
using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListMigrationJobsGetter(ListsDbContext dbContext)
    : IGetListMigrationJobs, IGetListMigrationJob
{
    private static Expression<Func<ListMigrationJobDb, ListMigrationJob>> Projector => job => new ListMigrationJob
    {
        Id = job.Id,
        ListId = job.ListId,
        BackupListId = job.BackupListId,
        BackupListName = job.BackupListName,
        RequestedByUserId = job.RequestedByUserId,
        RequestedOn = job.RequestedOn,
        PlanJson = job.PlanJson,
        Status = job.Status,
        StartedOn = job.StartedOn,
        CompletedOn = job.CompletedOn,
        FailedOn = job.FailedOn,
        CanceledOn = job.CanceledOn,
        BackupCompletedOn = job.BackupCompletedOn,
        LastProgressOn = job.LastProgressOn,
        ProgressPercent = job.ProgressPercent,
        TotalItems = job.TotalItems,
        ProcessedItems = job.ProcessedItems,
        CurrentMessage = job.CurrentMessage,
        FailureReason = job.FailureReason,
        CancelRequested = job.CancelRequested,
        CancelRequestedByUserId = job.CancelRequestedByUserId,
        CancelRequestedOn = job.CancelRequestedOn
    };

    public async Task<ListMigrationJob?> GetAsync(Guid listId, Guid jobId, CancellationToken cancellationToken)
    {
        return await dbContext.ListMigrationJobs
            .AsNoTracking()
            .Where(j => j.ListId == listId && j.Id == jobId)
            .Select(Projector)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ListMigrationJob[]> GetAsync(Guid listId, CancellationToken cancellationToken)
    {
        return await dbContext.ListMigrationJobs
            .AsNoTracking()
            .Where(j => j.ListId == listId)
            .OrderByDescending(j => j.RequestedOn)
            .Select(Projector)
            .ToArrayAsync(cancellationToken);
    }
}