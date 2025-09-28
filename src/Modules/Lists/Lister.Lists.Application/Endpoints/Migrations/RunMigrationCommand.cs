using Lister.Core.Application;

namespace Lister.Lists.Application.Endpoints.Migrations;

public enum MigrationMode
{
    DryRun,
    Execute
}

public record RunMigrationCommand(
    Guid ListId,
    MigrationPlan Plan,
    MigrationMode Mode
) : RequestBase<MigrationDryRunResult>;