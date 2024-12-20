using Lister.Core.Application.Behaviors;
using Lister.Core.Domain.Services;
using Lister.Core.Infrastructure.OpenAi.Services;
using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Application.Endpoints.ConvertTextToListItem;
using Lister.Lists.Application.Endpoints.CreateList;
using Lister.Lists.Application.Endpoints.CreateListItem;
using Lister.Lists.Application.Endpoints.DeleteList;
using Lister.Lists.Application.Endpoints.DeleteListItem;
using Lister.Lists.Application.Endpoints.GetItemDetails;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Events;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Configuration;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.Services;
using Lister.Users.Infrastructure.Sql;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
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
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        var applicationDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.ApplicationDbContextMigrationAssemblyName;
        services.AddDbContext<UsersDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(applicationDbContextMigrationAssemblyName)));

        var dataProtectionKeyDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.DataProtectionKeyDbContextMigrationAssemblyName;
        services.AddDbContext<DataProtectionKeyDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(dataProtectionKeyDbContextMigrationAssemblyName)));

        services.AddDataProtection()
            .SetApplicationName("Lister")
            .PersistKeysToDbContext<DataProtectionKeyDbContext>();

        var listsDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.ListsDbContextMigrationAssemblyName;
        services.AddDbContext<ListsDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(listsDbContextMigrationAssemblyName)));

        services.AddScoped<IListsUnitOfWork<ListDb, ItemDb>, ListsUnitOfWork>();
        services.AddScoped<IGetCompletedJson, CompletedJsonGetter>();
        services.AddScoped<IGetItemDetails, ItemDetailsGetter>();
        services.AddScoped<IGetListItemDefinition, ListItemDefinitionGetter>();
        services.AddScoped<IGetPagedList, PagedListGetter>();
        services.AddScoped<IGetListNames, ListNamesGetter>();

        services.AddAutoMapper(config =>
            config.AddProfile<CoreMappingProfile>());

        return services;
    }

    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ListsAggregate<ListDb, ItemDb>>();
        services.AddMediatR(config => { config.RegisterServicesFromAssemblyContaining<ListCreatedEvent>(); });
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => { config.RegisterServicesFromAssemblyContaining<GetItemDetailsQuery>(); });

        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(AssignUserBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        services.AddScoped(typeof(IRequestHandler<CreateListItemCommand, ListItem>),
            typeof(CreateListItemCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<ConvertTextToListItemCommand, ListItem>),
            typeof(ConvertTextToListItemCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<CreateListCommand, ListItemDefinition>),
            typeof(CreateListCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<DeleteListCommand>),
            typeof(DeleteListCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<DeleteListItemCommand>),
            typeof(DeleteListItemCommandHandler<ListDb, ItemDb>));
        return services;
    }
}