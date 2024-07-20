using Lister.Core;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Configuration;
using Lister.Core.SqlDB.Entities;
using Lister.Domain;
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
        services.AddScoped<IListerUnitOfWork<ListEntity, ItemEntity>, ListerUnitOfWork>();
        services.AddScoped<IListsStore<ListEntity, ItemEntity>, ListsStore>();
        services.AddAutoMapper(config =>
            config.AddProfile<CoreMappingProfile>());
        return services;
    }
    
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ListAggregate<ListEntity, ItemEntity>>();
        return services;
    }
}