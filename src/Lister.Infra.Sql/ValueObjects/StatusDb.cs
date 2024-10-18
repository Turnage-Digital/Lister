using Lister.Domain.ValueObjects;
using Lister.Infra.Sql.Entities;

namespace Lister.Infra.Sql.ValueObjects;

public record StatusDb : Status
{
    public int? Id { get; set; }

    public ListDb ListDb { get; set; } = null!;

    public Guid? ListId { get; set; }
}