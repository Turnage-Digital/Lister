namespace Lister.Lists.Domain.Enums;

public enum ListMigrationJobHistoryType
{
    Queued,
    StatusChanged,
    BackupCompleted,
    CancelRequested,
    CancelRequestCleared
}