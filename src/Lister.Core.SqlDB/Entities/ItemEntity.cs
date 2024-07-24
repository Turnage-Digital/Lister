using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB.Entities;

public record ItemEntity : Item
{
    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public ListEntity List { get; set; } = null!;

    public Guid? ListId { get; set; }
}