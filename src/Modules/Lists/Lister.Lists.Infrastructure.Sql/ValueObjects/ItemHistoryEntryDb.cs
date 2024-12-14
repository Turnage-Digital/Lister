using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql.ValueObjects;

public record ItemHistoryEntryDb : Entry<ItemHistoryType>
{
    public int? Id { get; set; }

    public int? ItemId { get; set; }

    public ItemDb? Item { get; set; }
}