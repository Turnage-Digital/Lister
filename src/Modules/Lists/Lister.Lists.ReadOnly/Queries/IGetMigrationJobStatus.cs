using Lister.Lists.Domain.Enums;

namespace Lister.Lists.ReadOnly.Queries;

public interface IGetMigrationJobStatus
{
    Task<MigrationJobStatusDto?> GetAsync(Guid listId, Guid correlationId, CancellationToken cancellationToken);
}

public record MigrationJobStatusDto(
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