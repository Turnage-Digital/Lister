namespace Lister.Core.Infrastructure.Sql;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = null!;

    public string MigrationAssemblyName { get; set; } = null!;
}