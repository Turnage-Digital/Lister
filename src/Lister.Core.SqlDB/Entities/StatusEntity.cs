using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB.Entities;

public record StatusEntity : Status
{
    public Guid? Id { get; set; }

    public ListEntity List { get; set; } = null!;

    public Guid? ListId { get; set; }
}