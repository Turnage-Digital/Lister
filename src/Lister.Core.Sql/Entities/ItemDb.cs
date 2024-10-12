using Lister.Core.Entities;

namespace Lister.Core.Sql.Entities;

public class ItemDb : Item
{
    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public ListDb ListDb { get; set; } = null!;

    public Guid? ListId { get; set; }
}