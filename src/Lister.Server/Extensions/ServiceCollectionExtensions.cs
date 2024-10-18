using Lister.Application.Validation;
using Lister.Domain;
using Lister.Infra.Sql;
using Lister.Infra.Sql.Configuration;
using Lister.Infra.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lister.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, Action<CoreConfiguration> configuration)
    {
        var coreConfiguration = new CoreConfiguration();
        configuration(coreConfiguration);
        return services.AddCore(coreConfiguration);
    }

    public static IServiceCollection AddCore(this IServiceCollection services, CoreConfiguration configuration)
    {
        var connectionString = configuration.DatabaseOptions.DefaultConnectionString;
        var migrationAssemblyName = configuration.DatabaseOptions.MigrationAssemblyName;
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName)));
        services.AddDbContext<ListerDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName)));
        services.AddScoped<IListerUnitOfWork<ListDb, ItemDb>, ListerUnitOfWork>();
        services.AddScoped<IListsStore<ListDb, ItemDb>, ListsStore>();
        services.AddAutoMapper(config => config.AddProfile<CoreMappingProfile>());
        return services;
    }

    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ListAggregate<ListDb, ItemDb>>();
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator, Validator>();
        return services;
    }
}