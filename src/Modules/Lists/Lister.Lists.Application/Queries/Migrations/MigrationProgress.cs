using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Application.Queries.Migrations;

public record MigrationProgress(
    Guid JobId,
    Guid SourceListId,
    Guid CorrelationId,
    ListMigrationJobStage Stage,
    string RequestedBy,
    DateTime CreatedOn,
    DateTime? StartedOn,
    DateTime? CompletedOn,
    Guid? BackupListId,
    Guid? NewListId,
    DateTime? BackupExpiresOn,
    DateTime? BackupRemovedOn,
    int Attempts,
    string? LastError
);