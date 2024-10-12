using Lister.Core.Sql.Entities;
using Lister.Core.Sql.ValueObjects;

namespace Lister.Core.Sql;

public class ListDb : IWritableList
{
    public string Name { get; set; } = null!;

    public string CreatedBy { get; init; } = null!;

    public DateTime CreatedOn { get; init; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public ICollection<ColumnDb> Columns { get; set; } = new HashSet<ColumnDb>();

    public ICollection<StatusDb> Statuses { get; set; } = new HashSet<StatusDb>();

    public ICollection<ItemDb> Items { get; set; } = new HashSet<ItemDb>();

    public Guid? Id { get; set; }
}