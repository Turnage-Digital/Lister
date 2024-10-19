using Lister.Application.Behaviors;
using Lister.Application.Queries.List;
using Lister.Application.Validation;
using Lister.Domain;
using Lister.Domain.Events.List;
using Lister.Infrastructure.Sql;
using Lister.Infrastructure.Sql.Configuration;
using Lister.Infrastructure.Sql.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        Action<InfrastructureConfiguration> configuration)
    {
        var coreConfiguration = new InfrastructureConfiguration();
        configuration(coreConfiguration);
        return services.AddInfrastructure(coreConfiguration);
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        InfrastructureConfiguration configuration)
    {
        var connectionString = configuration.DatabaseOptions.ConnectionString;
        var migrationAssemblyName = configuration.DatabaseOptions.MigrationAssemblyName;
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName)));
        services.AddDbContext<ListerDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName)));
        services.AddScoped<IListerUnitOfWork<ListDb>, ListerUnitOfWork>();
        services.AddScoped<IGetList<ListDb>, ListGetter>();
        services.AddScoped<IGetListItem, ListItemGetter>();
        services.AddScoped<IGetListItemDefinition, ListItemDefinitionGetter>();
        services.AddScoped<IGetListItems, ListItemsGetter>();
        services.AddScoped<IGetListNames, ListNamesGetter>();
        services.AddAutoMapper(config => config.AddProfile<CoreMappingProfile>());
        return services;
    }

    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ListAggregate<ListDb>>();
        services.AddMediatR(config => { config.RegisterServicesFromAssemblyContaining<ListCreatedEvent>(); });
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator, Validator>();
        services.AddMediatR(config => { config.RegisterServicesFromAssemblyContaining<GetListItemQuery>(); });
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(AssignUserBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));
        return services;
    }
}