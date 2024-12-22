using Lister.Core.Application.Behaviors;
using Lister.Core.Domain.Services;
using Lister.Core.Infrastructure.OpenAi.Services;
using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Application.Commands.ConvertTextToListItem;
using Lister.Lists.Application.Commands.CreateList;
using Lister.Lists.Application.Commands.CreateListItem;
using Lister.Lists.Application.Commands.DeleteList;
using Lister.Lists.Application.Commands.DeleteListItem;
using Lister.Lists.Application.Queries.GetItemDetails;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Events;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Configuration;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.Services;
using Lister.Server.Services;
using Lister.Users.Application.Behaviors;
using Lister.Users.Domain.Services;
using Lister.Users.Infrastructure.Sql;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace Lister.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Action<InfrastructureConfiguration> configuration
    )
    {
        var coreConfiguration = new InfrastructureConfiguration();
        configuration(coreConfiguration);
        return services.AddInfrastructure(coreConfiguration);
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        InfrastructureConfiguration configuration
    )
    {
        var connectionString = configuration.DatabaseOptions.ConnectionString;
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        
        /* Core */
        var dataProtectionKeyDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.DataProtectionKeyDbContextMigrationAssemblyName;
        services.AddDbContext<DataProtectionKeyDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(dataProtectionKeyDbContextMigrationAssemblyName)));

        services.AddDataProtection()
            .SetApplicationName("Lister")
            .PersistKeysToDbContext<DataProtectionKeyDbContext>();
        
        /* Users */
        var usersDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.UsersDbContextMigrationAssemblyName;
        services.AddDbContext<UsersDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(usersDbContextMigrationAssemblyName)));

        services.AddScoped<IGetCurrentUser, CurrentUserGetter>();
        
        /*
         *   "Identity": {
             "Password": {
               "RequireDigit": true,
               "RequiredLength": 8,
               "RequireNonAlphanumeric": true,
               "RequireLowercase": true,
               "RequireUppercase": true,
               "RequiredUniqueChars": 6
             },
             "Lockout": {
               "DefaultLockoutTimeSpan": "00:05:00",
               "MaxFailedAccessAttemptsBeforeLockout": 3
             },
             "SignIn": {
               "RequireConfirmedEmail": false
             }
           },
         */
        // services.AddDefaultIdentity<ApplicationUser>(options =>
        //         builder.Configuration.GetSection("Identity").Bind(options))
        //     .AddEntityFrameworkStores<UsersDbContext>();

        /* Lists */
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
        
        /* Automapper */
        services.AddAutoMapper(config =>
            config.AddProfile<ListsMappingProfile>());

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