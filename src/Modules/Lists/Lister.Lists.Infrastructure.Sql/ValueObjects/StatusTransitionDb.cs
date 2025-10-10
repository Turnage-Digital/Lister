using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql.ValueObjects;

public record StatusTransitionDb
{
    public int Id { get; set; }
    public Guid ListId { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public ListDb? ListDb { get; set; }
}