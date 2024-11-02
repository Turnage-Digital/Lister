using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Infrastructure.Sql.Entities;

public class ItemDb : Item
{
    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public ListDb ListDb { get; set; } = null!;
}