using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lister.Core.SqlDB.Entities;

[Table("Lists")]
public class ListEntity : IWritableList
{
    [Required]
    public string Name { get; set; } = null!;

    public ICollection<StatusEntity> Statuses { get; set; } = null!;

    public ICollection<ColumnEntity> Columns { get; set; } = null!;

    [Required]
    public string CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedOn { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid? Id { get; set; }
}