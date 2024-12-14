using System.Text.Json;
using System.Text.Json.Serialization;
using Lister.Core.Infrastructure.OpenAi;
using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql;
using Lister.Server.Extensions;
using Lister.Users.Infrastructure.Sql;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Lister.Server;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, config) => config
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level} {SourceContext}]{NewLine}{Message:lj}{NewLine}{NewLine}")
            .WriteTo.Seq(builder.Configuration["SeqUrl"]!)
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
        var applicationDbContextMigrationAssemblyName = typeof(UsersDbContext).Assembly.FullName!;
        var dataProtectionKeyDbContextMigrationAssemblyName = typeof(DataProtectionKeyDbContext).Assembly.FullName!;
        var listsDbContextMigrationAssemblyName = typeof(ListsDbContext).Assembly.FullName!;
        builder.Services.AddInfrastructure(config =>
        {
            config.DatabaseOptions.ConnectionString = connectionString;
            config.DatabaseOptions.ApplicationDbContextMigrationAssemblyName =
                applicationDbContextMigrationAssemblyName;
            config.DatabaseOptions.DataProtectionKeyDbContextMigrationAssemblyName =
                dataProtectionKeyDbContextMigrationAssemblyName;
            config.DatabaseOptions.ListsDbContextMigrationAssemblyName =
                listsDbContextMigrationAssemblyName;
        });
        builder.Services.AddDomain();
        builder.Services.AddApplication();

        builder.Services
            .AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<UsersDbContext>();

        builder.Services
            .ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(30);

                options.Events.OnRedirectToAccessDenied =
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
            });

        builder.Services.AddAuthorization();

        builder.Services.Configure<OpenAiOptions>(
            builder.Configuration.GetSection("OpenAI"));

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
        }

        app.UseHttpsRedirection();
        app.UseHsts();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        var identityGroup = app
            .MapGroup("/identity")
            .WithTags("Identity");

        identityGroup.MapIdentityApi<IdentityUser>();

        identityGroup.MapPost("logout",
            async (SignInManager<IdentityUser> signInManager) =>
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
}