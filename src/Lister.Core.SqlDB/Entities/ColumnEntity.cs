using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB.Entities;

[Table("Columns")]
public record ColumnEntity : Column
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid? Id { get; set; }
}