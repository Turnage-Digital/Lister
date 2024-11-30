using Lister.Lists.Domain.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;

namespace Lister.Lists.Infrastructure.Sql.Entities;

public class ListDb : IWritableList
{
    public Guid? Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public ICollection<ColumnDb> Columns { get; set; } = new HashSet<ColumnDb>();

    public ICollection<StatusDb> Statuses { get; set; } = new HashSet<StatusDb>();

    public ICollection<ItemDb> Items { get; set; } = new HashSet<ItemDb>();
    
    public ICollection<ListHistoryEntryDb> History { get; set; } = new HashSet<ListHistoryEntryDb>();
}