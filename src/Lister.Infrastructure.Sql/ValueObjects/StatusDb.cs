using Lister.Domain.ValueObjects;
using Lister.Infrastructure.Sql.Entities;

namespace Lister.Infrastructure.Sql.ValueObjects;

public record StatusDb : Status
{
    public int? Id { get; set; }

    public ListDb ListDb { get; set; } = null!;

    public Guid? ListId { get; set; }
}