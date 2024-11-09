using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql.ValueObjects;

public record ColumnDb : Column
{
    public int? Id { get; set; }

    public Guid? ListId { get; set; }

    public ListDb? ListDb { get; set; }  
}