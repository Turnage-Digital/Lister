using Lister.Core.ValueObjects;

namespace Lister.Core.Sql.ValueObjects;

public record StatusDb : Status
{
    public int? Id { get; set; }

    public ListDb ListDb { get; set; } = null!;

    public Guid? ListId { get; set; }
}