using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Infrastructure.Sql.Entities;

public class ItemDb : Item
{
    public ListDb ListDb { get; set; } = null!;
}