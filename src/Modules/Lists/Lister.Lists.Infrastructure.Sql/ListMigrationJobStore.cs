using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql;

public class ListMigrationJobStore(ListsDbContext dbContext)
    : IListMigrationJobStore<ListMigrationJobDb>
{
    public Task<ListMigrationJobDb> InitAsync(
        Guid listId,
        string requestedByUserId,
        string planJson,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;
        var job = new ListMigrationJobDb
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            RequestedByUserId = requestedByUserId,
            RequestedOn = now,
            PlanJson = planJson,
            Status = ListMigrationJobStatus.Queued,
            ProgressPercent = 0,
            TotalItems = 0,
            ProcessedItems = 0,
            CurrentMessage = "Queued for execution"
        };

        job.History.Add(new ListMigrationJobHistoryEntryDb
        {
            MigrationJob = job,
            Type = ListMigrationJobHistoryType.Queued,
            On = now,
            By = requestedByUserId,
            Bag = new { status = ListMigrationJobStatus.Queued }
        });

        return Task.FromResult(job);
    }

    public async Task CreateAsync(
        ListMigrationJobDb job,
        CancellationToken cancellationToken
    )
    {
        await dbContext.ListMigrationJobs.AddAsync(job, cancellationToken);
    }

    public Task<ListMigrationJobDb?> GetByIdAsync(Guid jobId, CancellationToken cancellationToken)
    {
        return dbContext.ListMigrationJobs
            .Include(j => j.History)
            .Where(j => j.Id == jobId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<ListMigrationJobDb?> GetActiveJobAsync(Guid listId, CancellationToken cancellationToken)
    {
        return dbContext.ListMigrationJobs
            .Include(j => j.History)
            .Where(j => j.ListId == listId)
            .Where(j => j.Status == ListMigrationJobStatus.Queued
                        || j.Status == ListMigrationJobStatus.PreparingBackup
                        || j.Status == ListMigrationJobStatus.Running)
            .OrderBy(j => j.RequestedOn)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<ListMigrationJobDb?> GetNextQueuedAsync(CancellationToken cancellationToken)
    {
        return dbContext.ListMigrationJobs
            .Include(j => j.History)
            .Where(j => j.Status == ListMigrationJobStatus.Queued)
            .OrderBy(j => j.RequestedOn)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> TryUpdateStatusAsync(
        ListMigrationJobDb job,
        ListMigrationJobStatus expectedStatus,
        ListMigrationJobStatus nextStatus,
        CancellationToken cancellationToken
    )
    {
        var updated = await dbContext.ListMigrationJobs
            .Where(j => j.Id == job.Id && j.Status == expectedStatus)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(j => j.Status, _ => nextStatus),
                cancellationToken);

        if (updated != 1)
        {
            await ReloadAsync(job, cancellationToken);
            return false;
        }

        job.Status = nextStatus;
        job.History.Add(new ListMigrationJobHistoryEntryDb
        {
            MigrationJob = job,
            Type = ListMigrationJobHistoryType.StatusChanged,
            On = DateTime.UtcNow,
            By = job.RequestedByUserId,
            Bag = new { status = nextStatus }
        });
        return true;
    }

    public Task SetStatusAsync(
        ListMigrationJobDb job,
        ListMigrationJobStatus status,
        CancellationToken cancellationToken
    )
    {
        job.Status = status;
        job.History.Add(new ListMigrationJobHistoryEntryDb
        {
            MigrationJob = job,
            Type = ListMigrationJobHistoryType.StatusChanged,
            On = DateTime.UtcNow,
            By = job.RequestedByUserId,
            Bag = new { status }
        });
        return Task.CompletedTask;
    }

    public Task<ListMigrationJobStatus> GetStatusAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.Status);
    }

    public Task SetStartedOnAsync(
        ListMigrationJobDb job,
        DateTime startedOn,
        CancellationToken cancellationToken
    )
    {
        job.StartedOn = startedOn;
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetStartedOnAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.StartedOn);
    }

    public Task SetLastProgressOnAsync(
        ListMigrationJobDb job,
        DateTime timestamp,
        CancellationToken cancellationToken
    )
    {
        job.LastProgressOn = timestamp;
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetLastProgressOnAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.LastProgressOn);
    }

    public Task SetBackupListAsync(
        ListMigrationJobDb job,
        Guid backupListId,
        string backupListName,
        CancellationToken cancellationToken
    )
    {
        job.BackupListId = backupListId;
        job.BackupListName = backupListName;
        return Task.CompletedTask;
    }

    public Task<(Guid? BackupListId, string? BackupListName)> GetBackupListAsync(
        ListMigrationJobDb job,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult((job.BackupListId, job.BackupListName));
    }

    public Task SetBackupCompletedOnAsync(
        ListMigrationJobDb job,
        DateTime completedOn,
        CancellationToken cancellationToken
    )
    {
        job.BackupCompletedOn = completedOn;
        job.History.Add(new ListMigrationJobHistoryEntryDb
        {
            MigrationJob = job,
            Type = ListMigrationJobHistoryType.BackupCompleted,
            On = completedOn,
            By = job.RequestedByUserId,
            Bag = new { job.BackupListId, job.BackupListName }
        });
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetBackupCompletedOnAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.BackupCompletedOn);
    }

    public Task SetCurrentMessageAsync(
        ListMigrationJobDb job,
        string message,
        CancellationToken cancellationToken
    )
    {
        job.CurrentMessage = message;
        return Task.CompletedTask;
    }

    public Task<string?> GetCurrentMessageAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.CurrentMessage);
    }

    public Task SetProgressCountsAsync(
        ListMigrationJobDb job,
        int processedItems,
        int totalItems,
        CancellationToken cancellationToken
    )
    {
        job.ProcessedItems = processedItems;
        job.TotalItems = totalItems;
        return Task.CompletedTask;
    }

    public Task<(int ProcessedItems, int TotalItems)> GetProgressCountsAsync(
        ListMigrationJobDb job,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult((job.ProcessedItems, job.TotalItems));
    }

    public Task SetProgressPercentAsync(
        ListMigrationJobDb job,
        int percent,
        CancellationToken cancellationToken
    )
    {
        job.ProgressPercent = percent;
        return Task.CompletedTask;
    }

    public Task<int?> GetProgressPercentAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.ProgressPercent);
    }

    public Task SetCompletedOnAsync(
        ListMigrationJobDb job,
        DateTime completedOn,
        CancellationToken cancellationToken
    )
    {
        job.CompletedOn = completedOn;
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetCompletedOnAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.CompletedOn);
    }

    public Task SetFailedOnAsync(
        ListMigrationJobDb job,
        DateTime failedOn,
        CancellationToken cancellationToken
    )
    {
        job.FailedOn = failedOn;
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetFailedOnAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.FailedOn);
    }

    public Task SetFailureReasonAsync(
        ListMigrationJobDb job,
        string? failureReason,
        CancellationToken cancellationToken
    )
    {
        job.FailureReason = failureReason;
        return Task.CompletedTask;
    }

    public Task<string?> GetFailureReasonAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.FailureReason);
    }

    public Task SetCancelRequestedAsync(
        ListMigrationJobDb job,
        bool cancelRequested,
        string? requestedByUserId,
        DateTime? requestedOn,
        CancellationToken cancellationToken
    )
    {
        job.CancelRequested = cancelRequested;
        job.CancelRequestedByUserId = requestedByUserId;
        job.CancelRequestedOn = requestedOn;
        job.History.Add(new ListMigrationJobHistoryEntryDb
        {
            MigrationJob = job,
            Type = cancelRequested
                ? ListMigrationJobHistoryType.CancelRequested
                : ListMigrationJobHistoryType.CancelRequestCleared,
            On = requestedOn ?? DateTime.UtcNow,
            By = requestedByUserId ?? job.RequestedByUserId,
            Bag = null
        });
        return Task.CompletedTask;
    }

    public Task<(bool CancelRequested, string? RequestedByUserId, DateTime? RequestedOn)> GetCancelRequestedAsync(
        ListMigrationJobDb job,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult((job.CancelRequested, job.CancelRequestedByUserId, job.CancelRequestedOn));
    }

    public Task SetCanceledOnAsync(
        ListMigrationJobDb job,
        DateTime canceledOn,
        CancellationToken cancellationToken
    )
    {
        job.CanceledOn = canceledOn;
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetCanceledOnAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.FromResult(job.CanceledOn);
    }

    private static Task ReloadAsync(ListMigrationJobDb job, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}