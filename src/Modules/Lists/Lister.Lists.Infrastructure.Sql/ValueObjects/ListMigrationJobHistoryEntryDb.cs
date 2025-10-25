using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql.ValueObjects;

public record ListMigrationJobHistoryEntryDb : Entry<ListMigrationJobHistoryType>
{
    public int? Id { get; set; }

    public Guid? MigrationJobId { get; set; }

    public ListMigrationJobDb? MigrationJob { get; set; }
}