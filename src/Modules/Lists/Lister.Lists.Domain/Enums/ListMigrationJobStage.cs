namespace Lister.Lists.Domain.Enums;

public enum ListMigrationJobStage
{
    Pending = 0,
    Running = 1,
    Completed = 2,
    Failed = 3,
    Archived = 4
}