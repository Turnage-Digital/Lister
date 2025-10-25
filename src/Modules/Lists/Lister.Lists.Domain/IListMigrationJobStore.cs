using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain;

public interface IListMigrationJobStore<TMigrationJob>
    where TMigrationJob : IWritableListMigrationJob
{
    Task<TMigrationJob> InitAsync(
        Guid listId,
        string requestedByUserId,
        string planJson,
        CancellationToken cancellationToken
    );

    Task CreateAsync(
        TMigrationJob job,
        CancellationToken cancellationToken
    );

    Task<TMigrationJob?> GetByIdAsync(Guid jobId, CancellationToken cancellationToken);

    Task<TMigrationJob?> GetActiveJobAsync(Guid listId, CancellationToken cancellationToken);

    Task<TMigrationJob?> GetNextQueuedAsync(CancellationToken cancellationToken);

    Task<bool> TryUpdateStatusAsync(
        TMigrationJob job,
        ListMigrationJobStatus expectedStatus,
        ListMigrationJobStatus nextStatus,
        CancellationToken cancellationToken
    );

    Task SetStatusAsync(
        TMigrationJob job,
        ListMigrationJobStatus status,
        CancellationToken cancellationToken
    );

    Task<ListMigrationJobStatus> GetStatusAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetStartedOnAsync(
        TMigrationJob job,
        DateTime startedOn,
        CancellationToken cancellationToken
    );

    Task<DateTime?> GetStartedOnAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetLastProgressOnAsync(
        TMigrationJob job,
        DateTime timestamp,
        CancellationToken cancellationToken
    );

    Task<DateTime?> GetLastProgressOnAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetBackupListAsync(
        TMigrationJob job,
        Guid backupListId,
        string backupListName,
        CancellationToken cancellationToken
    );

    Task<(Guid? BackupListId, string? BackupListName)> GetBackupListAsync(
        TMigrationJob job,
        CancellationToken cancellationToken
    );

    Task SetBackupCompletedOnAsync(
        TMigrationJob job,
        DateTime completedOn,
        CancellationToken cancellationToken
    );

    Task<DateTime?> GetBackupCompletedOnAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetCurrentMessageAsync(
        TMigrationJob job,
        string message,
        CancellationToken cancellationToken
    );

    Task<string?> GetCurrentMessageAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetProgressCountsAsync(
        TMigrationJob job,
        int processedItems,
        int totalItems,
        CancellationToken cancellationToken
    );

    Task<(int ProcessedItems, int TotalItems)> GetProgressCountsAsync(
        TMigrationJob job,
        CancellationToken cancellationToken
    );

    Task SetProgressPercentAsync(
        TMigrationJob job,
        int percent,
        CancellationToken cancellationToken
    );

    Task<int?> GetProgressPercentAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetCompletedOnAsync(
        TMigrationJob job,
        DateTime completedOn,
        CancellationToken cancellationToken
    );

    Task<DateTime?> GetCompletedOnAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetFailedOnAsync(
        TMigrationJob job,
        DateTime failedOn,
        CancellationToken cancellationToken
    );

    Task<DateTime?> GetFailedOnAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetFailureReasonAsync(
        TMigrationJob job,
        string? failureReason,
        CancellationToken cancellationToken
    );

    Task<string?> GetFailureReasonAsync(TMigrationJob job, CancellationToken cancellationToken);

    Task SetCancelRequestedAsync(
        TMigrationJob job,
        bool cancelRequested,
        string? requestedByUserId,
        DateTime? requestedOn,
        CancellationToken cancellationToken
    );

    Task<(bool CancelRequested, string? RequestedByUserId, DateTime? RequestedOn)> GetCancelRequestedAsync(
        TMigrationJob job,
        CancellationToken cancellationToken
    );

    Task SetCanceledOnAsync(
        TMigrationJob job,
        DateTime canceledOn,
        CancellationToken cancellationToken
    );

    Task<DateTime?> GetCanceledOnAsync(TMigrationJob job, CancellationToken cancellationToken);
}