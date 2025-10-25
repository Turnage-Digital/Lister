using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain.Views;

public record ListMigrationJob : IReadOnlyListMigrationJob
{
    public Guid? BackupListId { get; init; }
    public string? BackupListName { get; init; }
    public required string RequestedByUserId { get; init; }
    public required DateTime RequestedOn { get; init; }
    public required string PlanJson { get; init; }
    public required ListMigrationJobStatus Status { get; init; }
    public DateTime? StartedOn { get; init; }
    public DateTime? CompletedOn { get; init; }
    public DateTime? FailedOn { get; init; }
    public DateTime? CanceledOn { get; init; }
    public DateTime? BackupCompletedOn { get; init; }
    public DateTime? LastProgressOn { get; init; }
    public int? ProgressPercent { get; init; }
    public int TotalItems { get; init; }
    public int ProcessedItems { get; init; }
    public string? CurrentMessage { get; init; }
    public string? FailureReason { get; init; }
    public bool CancelRequested { get; init; }
    public string? CancelRequestedByUserId { get; init; }
    public DateTime? CancelRequestedOn { get; init; }
    public required Guid ListId { get; init; }
    public required Guid Id { get; init; }
}