using Lister.Core.Application;
using Lister.Lists.Application.Endpoints.Migrations.Shared;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Application.Endpoints.Migrations.RunMigration;

public enum MigrationMode
{
    DryRun,
    Execute
}

public record RunMigrationCommand(
    Guid ListId,
    MigrationPlan Plan,
    MigrationMode Mode
) : RequestBase<RunMigrationResponse>;

public record RunMigrationResponse(
    MigrationDryRunResult DryRun,
    MigrationJobSummary? Job,
    bool Enqueued
);

public record MigrationJobSummary(
    Guid Id,
    Guid ListId,
    string RequestedByUserId,
    ListMigrationJobStatus Status,
    DateTime RequestedOn,
    DateTime? StartedOn,
    DateTime? CompletedOn,
    DateTime? FailedOn,
    DateTime? CanceledOn,
    DateTime? LastProgressOn,
    int? ProgressPercent,
    int TotalItems,
    int ProcessedItems,
    string? CurrentMessage,
    string? FailureReason,
    bool CancelRequested
);

public record MigrationJobDetails(
    Guid Id,
    Guid ListId,
    string RequestedByUserId,
    ListMigrationJobStatus Status,
    DateTime RequestedOn,
    DateTime? StartedOn,
    DateTime? CompletedOn,
    DateTime? FailedOn,
    DateTime? CanceledOn,
    DateTime? BackupCompletedOn,
    DateTime? LastProgressOn,
    int? ProgressPercent,
    int TotalItems,
    int ProcessedItems,
    string? CurrentMessage,
    string? FailureReason,
    bool CancelRequested,
    string? CancelRequestedByUserId,
    DateTime? CancelRequestedOn,
    Guid? BackupListId,
    string? BackupListName,
    MigrationPlan? Plan
);