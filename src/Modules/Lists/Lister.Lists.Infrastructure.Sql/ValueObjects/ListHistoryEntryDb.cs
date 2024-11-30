using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql.ValueObjects;

public record ListHistoryEntryDb : Entry<ListHistoryType>
{
    public Guid? Id { get; set; }

    public Guid? ListId { get; set; }

    public ListDb? List { get; set; }
}