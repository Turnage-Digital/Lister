using Lister.Application.Validation;
using Lister.Core;
using Lister.Core.Sql;
using Lister.Core.Sql.Configuration;
using Lister.Core.Sql.Entities;
using Lister.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lister.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, string connectionString)
    {
        var migrationAssemblyName = typeof(ListerDbContext).Assembly.FullName!;
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