using Lister.Lists.Domain.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;

namespace Lister.Lists.Infrastructure.Sql.Entities;

public class ListDb : IWritableList
{
    public string Name { get; init; } = null!;

    public bool IsDeleted { get; set; }

    public ICollection<ColumnDb> Columns { get; set; } = new HashSet<ColumnDb>();

    public ICollection<StatusDb> Statuses { get; set; } = new HashSet<StatusDb>();

    public ICollection<ItemDb> Items { get; init; } = new HashSet<ItemDb>();

    public ICollection<ListHistoryEntryDb> History { get; init; } = new HashSet<ListHistoryEntryDb>();
    
    public Guid? Id { get; set; }
}