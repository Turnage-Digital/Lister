using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql.ValueObjects;

namespace Lister.Lists.Infrastructure.Sql.Entities;

public class ListMigrationJobDb : IWritableListMigrationJob
{
    public Guid? BackupListId { get; set; }

    public string? BackupListName { get; set; }

    public string RequestedByUserId { get; set; } = string.Empty;

    public DateTime RequestedOn { get; set; }

    public string PlanJson { get; set; } = string.Empty;

    public ListMigrationJobStatus Status { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public DateTime? FailedOn { get; set; }

    public DateTime? CanceledOn { get; set; }

    public DateTime? BackupCompletedOn { get; set; }

    public DateTime? LastProgressOn { get; set; }

    public int? ProgressPercent { get; set; }

    public int TotalItems { get; set; }

    public int ProcessedItems { get; set; }

    public string? CurrentMessage { get; set; }

    public string? FailureReason { get; set; }

    public bool CancelRequested { get; set; }

    public string? CancelRequestedByUserId { get; set; }

    public DateTime? CancelRequestedOn { get; set; }

    public ICollection<ListMigrationJobHistoryEntryDb> History { get; init; } =
        new HashSet<ListMigrationJobHistoryEntryDb>();

    public Guid Id { get; set; }

    public Guid ListId { get; set; }
}