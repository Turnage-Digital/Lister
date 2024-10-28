namespace Lister.Core.Infrastructure.Sql;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = null!;

    public string ListerDbContextMigrationAssemblyName { get; set; } = null!;

    public string ApplicationDbContextMigrationAssemblyName { get; set; } = null!;
}