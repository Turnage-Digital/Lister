namespace Lister.Lists.Domain.Enums;

public enum ListMigrationJobStatus
{
    Queued = 0,
    PreparingBackup = 1,
    Running = 2,
    Completed = 3,
    Failed = 4,
    Canceled = 5
}