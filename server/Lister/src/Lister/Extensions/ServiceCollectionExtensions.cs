using Lister.Core;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Configuration;
using Lister.Core.SqlDB.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lister.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, string connectionString)
    {
        var migrationAssemblyName = typeof(ListerDbContext).Assembly.FullName!;
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, serverVersion,
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName)));
        services.AddDbContext<ListerDbContext>(options =>
            options.UseMySql(connectionString, serverVersion,
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName)));
        services.AddScoped<IListerUnitOfWork<ListEntity>, ListerUnitOfWork>();
        services.AddStores();
        services.AddAutoMapper(config =>
            config.AddProfile<CoreMappingProfile>());
        return services;
    }

    private static IServiceCollection AddStores(this IServiceCollection services)
    {
        services.AddScoped<IListsStore<ListEntity>, ListsStore>();
        return services;
    }
}