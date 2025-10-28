using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Infrastructure.Sql.Entities;

public class ListMigrationJobDb
{
    public Guid Id { get; set; }
    public Guid SourceListId { get; set; }
    public string RequestedBy { get; set; } = null!;
    public string PlanJson { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime? StartedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public DateTime? BackupRemovedOn { get; set; }
    public int Attempts { get; set; }
    public string? LastError { get; set; }
    public DateTime? AvailableAfter { get; set; }
    public Guid CorrelationId { get; set; }
    public Guid? BackupListId { get; set; }
    public Guid? NewListId { get; set; }
    public DateTime? BackupExpiresOn { get; set; }
    public ListMigrationJobStage Stage { get; set; }
}
