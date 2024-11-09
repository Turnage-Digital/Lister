using Lister.Lists.Domain.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;

namespace Lister.Lists.Infrastructure.Sql.Entities;

public class ItemDb : Item
{
    public bool IsDeleted { get; set; }

    public ListDb List { get; set; } = null!;
    
    public ICollection<ItemHistoryEntryDb> History { get; set; } = new HashSet<ItemHistoryEntryDb>();
}