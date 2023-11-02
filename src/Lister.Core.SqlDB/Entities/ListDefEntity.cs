using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lister.Core.SqlDB.Entities;

[Table("ListDefs")]
public class ListDefEntity : IWritableListDef
{
    [Required]
    public string Name { get; set; } = null!;

    public ICollection<StatusDefEntity> StatusDefs { get; set; } = null!;

    public ICollection<ColumnDefEntity> ColumnDefs { get; set; } = null!;
    
    [Required]
    public string CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedOn { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid? Id { get; set; }
}