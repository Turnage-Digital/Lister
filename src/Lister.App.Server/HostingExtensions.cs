using System.Text.Json;
using System.Text.Json.Serialization;
using Lister.App.Server.Services;
using Lister.Core.Application.Behaviors;
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
using Lister.Lists.Domain;
using Lister.Lists.Domain.Events;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Configuration;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.Services;
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

namespace Lister.App.Server;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, config) => config
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level} {SourceContext}]{NewLine}{Message:lj}{NewLine}{NewLine}")
            .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
            .Enrich.FromLogContext());

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        var usersDbContextMigrationAssemblyName = typeof(UsersDbContext).Assembly.FullName!;
        var dataProtectionKeyDbContextMigrationAssemblyName = typeof(DataProtectionKeyDbContext).Assembly.FullName!;
        var listsDbContextMigrationAssemblyName = typeof(ListsDbContext).Assembly.FullName!;
        builder.Services.AddInfrastructure(config =>
        {
            config.DatabaseOptions.ConnectionString = connectionString;
            config.DatabaseOptions.UsersDbContextMigrationAssemblyName =
                usersDbContextMigrationAssemblyName;
            config.DatabaseOptions.DataProtectionKeyDbContextMigrationAssemblyName =
                dataProtectionKeyDbContextMigrationAssemblyName;
            config.DatabaseOptions.ListsDbContextMigrationAssemblyName =
                listsDbContextMigrationAssemblyName;
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
            .PersistKeysToDbContext<DataProtectionKeyDbContext>();

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
        var dataProtectionKeyDbContextMigrationAssemblyName =
            configuration.DatabaseOptions.DataProtectionKeyDbContextMigrationAssemblyName;
        services.AddDbContext<DataProtectionKeyDbContext>(options => options.UseMySql(connectionString, serverVersion,
            optionsBuilder => optionsBuilder.MigrationsAssembly(dataProtectionKeyDbContextMigrationAssemblyName)));

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

        /* Automapper */
        services.AddAutoMapper(config =>
            config.AddProfile<ListsMappingProfile>());

        return services;
    }

    private static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ListsAggregate<ListDb, ItemDb>>();
        services.AddMediatR(config => { config.RegisterServicesFromAssemblyContaining<ListCreatedEvent>(); });
        return services;
    }

    private static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => { config.RegisterServicesFromAssemblyContaining<GetItemDetailsQuery>(); });
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(AssignUserBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));
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
        return services;
    }

    private class InfrastructureConfiguration
    {
        public DatabaseOptions DatabaseOptions { get; } = new();
    }
}