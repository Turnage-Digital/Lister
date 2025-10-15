using System.Text.Json;
using System.Text.Json.Serialization;
using Lister.App.Server.Services;
using Lister.Core.Application.Behaviors;
using Lister.Core.Domain;
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
using Lister.Notifications.Domain;
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

            config.WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level} {SourceContext}]{NewLine}{Message:lj}{NewLine}{NewLine}")
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Error)
                .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
                .Enrich.FromLogContext();

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                if (!string.IsNullOrWhiteSpace(seqApiKey))
                {
                    config.WriteTo.Seq(seqUrl, apiKey: seqApiKey);
                }
                else
                {
                    config.WriteTo.Seq(seqUrl);
                }
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
        builder.Services.AddInfrastructure(connectionString);
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
        string connectionString
    )
    {
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        /* Core */
        services.AddDbContextWithMigrations<CoreDbContext>(connectionString, serverVersion);
        services.AddScoped<IDomainEventQueue, DomainEventQueue>();

        /* Users */
        services.AddDbContextWithMigrations<UsersDbContext>(connectionString, serverVersion);
        services.AddScoped<IGetCurrentUser, CurrentUserGetter>();

        /* Lists */
        services.AddDbContextWithMigrations<ListsDbContext>(connectionString, serverVersion);
        services.AddScoped<IListsUnitOfWork<ListDb, ItemDb>, ListsUnitOfWork>();
        services.AddScoped<IGetCompletedJson, CompletedJsonGetter>();
        services.AddScoped<IGetItemDetails, ItemDetailsGetter>();
        services.AddScoped<IGetListItemDefinition, ListItemDefinitionGetter>();
        services.AddScoped<IGetPagedList, PagedListGetter>();
        services.AddScoped<IGetListNames, ListNamesGetter>();
        services.AddScoped<IGetListHistory, ListHistoryGetter>();
        services.AddScoped<IGetItemHistory, ItemHistoryGetter>();

        /* Notifications */
        services.AddDbContextWithMigrations<NotificationsDbContext>(connectionString, serverVersion);
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
        services.AddScoped<IValidateListItemBag<ListDb>, ListItemBagValidator<ListDb, ItemDb>>();
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

        services.AddScoped<IMigrationValidator, MigrationValidator>();
        services.AddScoped<MigrationExecutor<ListDb, ItemDb>>();

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
        
        // Background workers
        services.AddHostedService<NotificationDeliveryService>();
        services.AddHostedService<OutboxDispatcherService>();
        return services;
    }

    private static IServiceCollection AddDbContextWithMigrations<TContext>(
        this IServiceCollection services,
        string connectionString,
        ServerVersion serverVersion
    ) where TContext : DbContext
    {
        var migrationsAssembly = typeof(TContext).Assembly.FullName!;
        services.AddDbContext<TContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly)));
        return services;
    }
}