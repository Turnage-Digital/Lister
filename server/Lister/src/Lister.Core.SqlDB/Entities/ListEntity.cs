namespace Lister.Core.SqlDB.Entities;

public class ListEntity : IWritableList
{
    public string Name { get; set; } = null!;

    public string CreatedBy { get; init; } = null!;

    public DateTime CreatedOn { get; init; }
    
    public string? DeletedBy { get; set; }
    
    public DateTime? DeletedOn { get; set; }

    public ICollection<ColumnEntity> Columns { get; set; } = new HashSet<ColumnEntity>();

    public ICollection<StatusEntity> Statuses { get; set; } = new HashSet<StatusEntity>();

    public ICollection<ItemEntity> Items { get; set; } = new HashSet<ItemEntity>();

    public Guid? Id { get; set; }
}