using Lister.Core.Application.Behaviors;
using Lister.Core.Application.Validation;
using Lister.Lists.Application.Commands;
using Lister.Lists.Application.Commands.Handlers;
using Lister.Lists.Application.Queries;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Events;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.OpenAi;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Configuration;
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
        services.AddScoped<IGetCompletedJson, CompletedJsonGetter>();
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
        services.AddScoped(typeof(IRequestHandler<AddListItemCommand, Item>),
            typeof(AddListItemCommandHandler<ListDb>));
        services.AddScoped(typeof(IRequestHandler<ConvertTextToListItemCommand, Item>),
            typeof(ConvertTextToListItemCommandHandler<ListDb>));
        services.AddScoped(typeof(IRequestHandler<CreateListCommand, ListItemDefinition>),
            typeof(CreateListCommandHandler<ListDb>));
        services.AddScoped(typeof(IRequestHandler<DeleteListCommand>), typeof(DeleteListCommandHandler<ListDb>));
        return services;
    }
}