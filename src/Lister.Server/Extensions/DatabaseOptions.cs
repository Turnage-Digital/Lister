namespace Lister.Core;

public class DatabaseOptions
{
    public string DefaultConnectionString { get; set; } = null!;

    public string MigrationAssemblyName { get; set; } = null!;
}