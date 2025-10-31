using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Commands.Migrations;

public class MigrationResult
{
    public bool IsSafe { get; init; }
    public string[] Messages { get; init; } = [];
    public MigrationPlan? SuggestedPlan { get; init; }
    public Guid? CorrelationId { get; init; }
}