using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB.Entities;

public record ColumnEntity : Column
{
    public int? Id { get; set; }

    public ListEntity List { get; set; } = null!;

    public Guid? ListId { get; set; }
}