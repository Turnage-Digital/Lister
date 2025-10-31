using System.ComponentModel.DataAnnotations;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Endpoints.Commands.Migrations.RunMigration;

public class RunMigrationRequest
{
    [Required]
    public MigrationPlan Plan { get; init; } = null!;

    public MigrationMode Mode { get; init; } = MigrationMode.Execute;
}