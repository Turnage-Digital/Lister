using System.Text.Json;
using System.Text.Json.Serialization;
using Lister.App.Server.Integration;
using Lister.App.Server.Services;
using Lister.Core.Application.Behaviors;
using Lister.Core.Domain;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Core.Domain.Services;
using Lister.Core.Infrastructure.OpenAi;
using Lister.Core.Infrastructure.OpenAi.Services;
using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Application.Endpoints.ConvertTextToListItem;
using Lister.Lists.Application.Endpoints.CreateList;
using Lister.Lists.Application.Endpoints.CreateListItem;
using Lister.Lists.Application.Endpoints.DeleteList;
using Lister.Lists.Application.Endpoints.DeleteListItem;
using Lister.Lists.Application.Endpoints.GetItemDetails;
using Lister.Lists.Application.Endpoints.GetStatusTransitions;
using Lister.Lists.Application.Endpoints.Migrations;
using Lister.Lists.Application.Endpoints.UpdateList;
using Lister.Lists.Application.Endpoints.UpdateListItem;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Configuration;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.Services;
using Lister.Notifications.Application.Endpoints.CreateNotificationRule;
using Lister.Notifications.Application.Endpoints.DeleteNotificationRule;
using Lister.Notifications.Application.Endpoints.GetNotificationDetails;
using Lister.Notifications.Application.Endpoints.GetUnreadNotificationCount;
using Lister.Notifications.Application.Endpoints.GetUserNotifications;
using Lister.Notifications.Application.Endpoints.MarkAllNotificationsAsRead;
using Lister.Notifications.Application.Endpoints.MarkNotificationAsRead;
using Lister.Notifications.Application.Endpoints.UpdateNotificationRule;
using Lister.Notifications.Application.EventHandlers.ListItemCreated;
using Lister.Notifications.Domain;
using Lister.Notifications.Domain.Events;
using Lister.Notifications.Domain.Services;
using Lister.Notifications.Domain.Views;
using Lister.Notifications.Infrastructure.Sql;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Lister.Notifications.Infrastructure.Sql.Services;
using Lister.Users.Application.Behaviors;
using Lister.Users.Domain.Entities;
using Lister.Users.Domain.Services;
using Lister.Users.Infrastructure.Sql;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace Lister.App.Server;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, config) =>
        {
            var seqUrl = ctx.Configuration["Seq:Url"];
            var seqApiKey = ctx.Configuration["Seq:ApiKey"];

            config
            // // Temporarily quiet EF Core logs for clearer runtime sampling
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Error)
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level} {SourceContext}]{NewLine}{Message:lj}{NewLine}{NewLine}")
            .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
            .Enrich.FromLogContext();

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                if (!string.IsNullOrWhiteSpace(seqApiKey))
                    config.WriteTo.Seq(seqUrl, apiKey: seqApiKey);
                else
                    config.WriteTo.Seq(seqUrl);
            }
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSingleton<ChangeFeed>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        var usersDbContextMigrationAssemblyName = typeof(UsersDbContext).Assembly.FullName!;
        var coreDbContextMigrationAssemblyName = typeof(CoreDbContext).Assembly.FullName!;
        var listsDbContextMigrationAssemblyName = typeof(ListsDbContext).Assembly.FullName!;
        var notificationsDbContextMigrationAssemblyName = typeof(NotificationsDbContext).Assembly.FullName!;
        builder.Services.AddInfrastructure(config =>
        {
            config.DatabaseOptions.ConnectionString = connectionString;
            config.DatabaseOptions.UsersDbContextMigrationAssemblyName =
                usersDbContextMigrationAssemblyName;
            config.DatabaseOptions.CoreDbContextMigrationAssemblyName =
                coreDbContextMigrationAssemblyName;
            config.DatabaseOptions.ListsDbContextMigrationAssemblyName =
                listsDbContextMigrationAssemblyName;
            config.DatabaseOptions.NotificationsDbContextMigrationAssemblyName =
                notificationsDbContextMigrationAssemblyName;
        });
        builder.Services.AddDomain();
        builder.Services.AddApplication();

        builder.Services
            .AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddTransient<IEmailSender, StubEmailSender>();

        builder.Services
            .ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

        builder.Services.AddAuthorization();

        builder.Services.AddDataProtection()
            .SetApplicationName("Lister")
            .PersistKeysToDbContext<CoreDbContext>();

        builder.Services.Configure<OpenAiOptions>(
            builder.Configuration.GetSection("OpenAi"));

        if (builder.Environment.IsDevelopment())
        {
            builder.Services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Lister API"
                    });
                });
        }

        var retval = builder.Build();
        return retval;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler();
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        var identityGroup = app
            .MapGroup("/identity")
            .WithTags("Identity");

        identityGroup.MapIdentityApi<User>();

        identityGroup.MapPost("logout",
            async (SignInManager<User> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            }
        );

#if DEBUG
        SeedData.EnsureSeedData(app);
#endif

        return app;
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Action<InfrastructureConfiguration> configuration
    )
    {
        var coreConfiguration = new InfrastructureConfiguration();
        configuration(coreConfiguration);
        return services.AddInfrastructure(coreConfiguration);
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        InfrastructureConfiguration configuration
    )
    {
        var connectionString = configuration.DatabaseOptions.ConnectionString;
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        /* Core */
        var coreDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.CoreDbContextMigrationAssemblyName;
        services.AddDbContext<CoreDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(coreDbContextMigrationAssemblyName)));
        services.AddScoped<IDomainEventQueue, DomainEventQueue>();

        /* Users */
        var usersDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.UsersDbContextMigrationAssemblyName;
        services.AddDbContext<UsersDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(usersDbContextMigrationAssemblyName)));
        services.AddScoped<IGetCurrentUser, CurrentUserGetter>();

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

        /* Notifications */
        var notificationsDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.NotificationsDbContextMigrationAssemblyName;
        services.AddDbContext<NotificationsDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(notificationsDbContextMigrationAssemblyName)));
        services.AddScoped<INotificationsUnitOfWork<NotificationRuleDb, NotificationDb>, NotificationsUnitOfWork>();
        services.AddScoped<IGetUserNotifications, UserNotificationsGetter>();
        services.AddScoped<IGetNotificationDetails, NotificationDetailsGetter>();
        services.AddScoped<IGetUserNotificationRules, UserNotificationRulesGetter>();
        services.AddScoped<IGetUnreadNotificationCount, UnreadNotificationCountGetter>();
        services.AddScoped<IGetActiveNotificationRules, ActiveNotificationRulesGetter>();
        services.AddScoped<IGetPendingNotifications, PendingNotificationsGetter>();

        /* Automapper */
        services.AddAutoMapper(config =>
            config.AddProfile<ListsMappingProfile>());

        return services;
    }

    private static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ListsAggregate<ListDb, ItemDb>>();
        services.AddScoped<NotificationAggregate<NotificationRuleDb, NotificationDb>>();
        return services;
    }

    private static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<GetItemDetailsQuery>();
            config.RegisterServicesFromAssemblyContaining<GetNotificationDetailsQuery>();
        });

        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(AssignUserBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        // Lists - close generic handlers in composition root
        services.AddScoped(typeof(IRequestHandler<ConvertTextToListItemCommand, ListItem>),
            typeof(ConvertTextToListItemCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<CreateListCommand, ListItemDefinition>),
            typeof(CreateListCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<CreateListItemCommand, ListItem>),
            typeof(CreateListItemCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<DeleteListCommand>),
            typeof(DeleteListCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<DeleteListItemCommand>),
            typeof(DeleteListItemCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<UpdateListItemCommand>),
            typeof(UpdateListItemCommandHandler<ListDb, ItemDb>));
        services.AddScoped<IMigrationValidator, MigrationValidator>();
        services.AddScoped<MigrationExecutor<ListDb, ItemDb>>();
        services.AddScoped(typeof(IRequestHandler<RunMigrationCommand, MigrationDryRunResult>),
            typeof(RunMigrationCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<GetStatusTransitionsQuery, StatusTransition[]>),
            typeof(GetStatusTransitionsQueryHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<UpdateListCommand>),
            typeof(UpdateListCommandHandler<ListDb, ItemDb>));
        services.AddScoped(typeof(IRequestHandler<UpdateListItemCommand>),
            typeof(UpdateListItemCommandHandler<ListDb, ItemDb>));

        // Notifications - close generic handlers in composition root
        services.AddScoped(typeof(IRequestHandler<CreateNotificationRuleCommand, NotificationRule>),
            typeof(CreateNotificationRuleCommandHandler<NotificationRuleDb, NotificationDb>));
        services.AddScoped(typeof(IRequestHandler<UpdateNotificationRuleCommand>),
            typeof(UpdateNotificationRuleCommandHandler<NotificationRuleDb, NotificationDb>));
        services.AddScoped(typeof(IRequestHandler<DeleteNotificationRuleCommand>),
            typeof(DeleteNotificationRuleCommandHandler<NotificationRuleDb, NotificationDb>));
        services.AddScoped(typeof(IRequestHandler<MarkNotificationAsReadCommand>),
            typeof(MarkNotificationAsReadCommandHandler<NotificationRuleDb, NotificationDb>));
        services.AddScoped(typeof(IRequestHandler<MarkAllNotificationsAsReadCommand>),
            typeof(MarkAllNotificationsAsReadCommandHandler<NotificationRuleDb, NotificationDb>));
        services.AddScoped(typeof(IRequestHandler<GetNotificationDetailsQuery, NotificationDetails?>),
            typeof(GetNotificationDetailsQueryHandler<NotificationRuleDb, NotificationDb>));
        services.AddScoped(typeof(IRequestHandler<GetUserNotificationsQuery, NotificationListPage>),
            typeof(GetUserNotificationsQueryHandler<NotificationRuleDb, NotificationDb>));
        services.AddScoped(typeof(IRequestHandler<GetUnreadNotificationCountQuery, int>),
            typeof(GetUnreadNotificationCountQueryHandler<NotificationRuleDb, NotificationDb>));

        // Notifications integration event handlers (use namespace qualifier as requested)
        services.AddScoped<INotificationHandler<ListItemCreatedIntegrationEvent>,
            NotifyEventHandler<NotificationRuleDb, NotificationDb>>();
        services
            .AddScoped<INotificationHandler<ListItemDeletedIntegrationEvent>, Notifications.Application.EventHandlers.
                ListItemDeleted.NotifyEventHandler<NotificationRuleDb, NotificationDb>>();
        services
            .AddScoped<INotificationHandler<ListDeletedIntegrationEvent>, Notifications.Application.EventHandlers.
                ListDeleted.NotifyEventHandler<NotificationRuleDb, NotificationDb>>();

        // Change feed event stream handlers
        services.AddScoped<INotificationHandler<ListItemCreatedIntegrationEvent>, ListItemCreatedStreamHandler>();
        services.AddScoped<INotificationHandler<ListItemDeletedIntegrationEvent>, ListItemDeletedStreamHandler>();
        services.AddScoped<INotificationHandler<ListDeletedIntegrationEvent>, ListDeletedStreamHandler>();
        services.AddScoped<INotificationHandler<ListUpdatedIntegrationEvent>, ListUpdatedStreamHandler>();
        services
            .AddScoped<INotificationHandler<ListMigrationStartedIntegrationEvent>, ListMigrationStartedStreamHandler>();
        services
            .AddScoped<INotificationHandler<ListMigrationProgressIntegrationEvent>,
                ListMigrationProgressStreamHandler>();
        services
            .AddScoped<INotificationHandler<ListMigrationCompletedIntegrationEvent>,
                ListMigrationCompletedStreamHandler>();
        services
            .AddScoped<INotificationHandler<ListMigrationFailedIntegrationEvent>, ListMigrationFailedStreamHandler>();

        // Outbox handlers (persist events for durability)
        services.AddScoped<INotificationHandler<ListItemCreatedIntegrationEvent>, ListItemCreatedOutboxHandler>();
        services.AddScoped<INotificationHandler<ListItemDeletedIntegrationEvent>, ListItemDeletedOutboxHandler>();
        services.AddScoped<INotificationHandler<ListDeletedIntegrationEvent>, ListDeletedOutboxHandler>();
        services.AddScoped<INotificationHandler<ListUpdatedIntegrationEvent>, ListUpdatedOutboxHandler>();
        services
            .AddScoped<INotificationHandler<ListMigrationStartedIntegrationEvent>, ListMigrationStartedOutboxHandler>();
        services
            .AddScoped<INotificationHandler<ListMigrationProgressIntegrationEvent>,
                ListMigrationProgressOutboxHandler>();
        services
            .AddScoped<INotificationHandler<ListMigrationCompletedIntegrationEvent>,
                ListMigrationCompletedOutboxHandler>();
        services
            .AddScoped<INotificationHandler<ListMigrationFailedIntegrationEvent>, ListMigrationFailedOutboxHandler>();
        services.AddScoped<INotificationHandler<NotificationCreatedEvent>, NotificationCreatedOutboxHandler>();
        services.AddScoped<INotificationHandler<NotificationProcessedEvent>, NotificationProcessedOutboxHandler>();
        services.AddScoped<INotificationHandler<NotificationReadEvent>, NotificationReadOutboxHandler>();
        services.AddScoped<INotificationHandler<AllNotificationsReadEvent>, AllNotificationsReadOutboxHandler>();
        services.AddScoped<INotificationHandler<NotificationDeliveryAttemptedEvent>,
            NotificationDeliveryAttemptedOutboxHandler>();
        services.AddScoped<INotificationHandler<NotificationRuleCreatedEvent>, NotificationRuleCreatedOutboxHandler>();
        services.AddScoped<INotificationHandler<NotificationRuleUpdatedEvent>, NotificationRuleUpdatedOutboxHandler>();
        services.AddScoped<INotificationHandler<NotificationRuleDeletedEvent>, NotificationRuleDeletedOutboxHandler>();

        // Background workers
        services.AddHostedService<NotificationDeliveryService>();
        services.AddHostedService<OutboxDispatcherService>();
        return services;
    }

    private class InfrastructureConfiguration
    {
        public DatabaseOptions DatabaseOptions { get; } = new();
    }
}
