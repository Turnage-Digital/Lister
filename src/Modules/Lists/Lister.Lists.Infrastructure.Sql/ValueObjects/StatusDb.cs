using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Infrastructure.Sql.ValueObjects;

public record StatusDb : Status
{
    public int? Id { get; set; }

    public ListDb ListDb { get; set; } = null!;

    public Guid? ListId { get; set; }
}