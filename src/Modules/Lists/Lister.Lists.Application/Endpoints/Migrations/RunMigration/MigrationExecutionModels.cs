namespace Lister.Lists.Application.Endpoints.Migrations.RunMigration;

public record MigrationProgressSnapshot(
    string Message,
    int Percent,
    int ProcessedItems,
    int TotalItems
);

public record MigrationExecutionResult(
    int TotalProcessedItems,
    int TotalItems
);