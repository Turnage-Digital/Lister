namespace Lister.Core.Infrastructure.Sql;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = null!;

    public string CoreDbContextMigrationAssemblyName { get; set; } = null!;

    public string ListsDbContextMigrationAssemblyName { get; set; } = null!;

    public string NotificationsDbContextMigrationAssemblyName { get; set; } = null!;

    public string UsersDbContextMigrationAssemblyName { get; set; } = null!;
}