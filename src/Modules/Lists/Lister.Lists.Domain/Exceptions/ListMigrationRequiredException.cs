using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Domain.Exceptions;

public class ListMigrationRequiredException : InvalidOperationException
{
    public ListMigrationRequiredException(IEnumerable<string> reasons, MigrationPlan? plan)
        : base("This update requires a list migration.")
    {
        Reasons = reasons?.ToArray() ?? [];
        Plan = plan;
    }

    public IReadOnlyList<string> Reasons { get; }

    public MigrationPlan? Plan { get; }
}