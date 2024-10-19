using Lister.Infrastructure.Sql;

namespace Lister.Server.Extensions;

public class InfrastructureConfiguration
{
    public DatabaseOptions DatabaseOptions { get; set; } = new();
}