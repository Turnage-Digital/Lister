using Lister.Core.Application;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Endpoints.Commands.Migrations.RunMigration;

public enum MigrationMode
{
    DryRun,
    Execute
}

public record RunMigrationCommand(
    Guid ListId,
    MigrationPlan Plan,
    MigrationMode Mode
) : RequestBase<MigrationResult>;