namespace Lister.Core.SqlDB.Entities;

public class ListEntity : IWritableList
{
    public string Name { get; set; } = null!;

    public ICollection<ColumnEntity> Columns { get; set; } = null!;

    public ICollection<StatusEntity> Statuses { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }
    
    public Guid? Id { get; set; }
}