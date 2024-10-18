namespace Lister.Server.Extensions;

public class DatabaseOptions
{
    public string DefaultConnectionString { get; set; } = null!;

    public string MigrationAssemblyName { get; set; } = null!;
}