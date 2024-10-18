using Lister.Domain.Entities;

namespace Lister.Infra.Sql.Entities;

public class ItemDb : Item
{
    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public ListDb ListDb { get; set; } = null!;

    public Guid? ListId { get; set; }
}