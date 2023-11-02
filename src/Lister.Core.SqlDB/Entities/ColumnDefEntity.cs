using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB.Entities;

[Table("ColumnDefs")]
public record ColumnDefEntity : ColumnDef
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid? Id { get; set; }
}