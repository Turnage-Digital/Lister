using System.ComponentModel.DataAnnotations;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Commands.Migrations;

public class RunMigrationRequest
{
    [Required]
    public MigrationPlan Plan { get; init; } = null!;

    public MigrationMode Mode { get; init; } = MigrationMode.Execute;
}