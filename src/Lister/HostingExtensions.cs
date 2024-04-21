using Lamar.Microsoft.DependencyInjection;
using Lister.Behaviors;
using Lister.Core.SqlDB;
using Lister.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

namespace Lister;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) => config
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(context.Configuration));

        builder.Host.UseLamar(registry =>
        {
            registry.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            // registry.Configure<JsonOptions>(options =>
            //     options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
            registry.AddCore(connectionString);

            registry.AddMediatR(config =>
                config.RegisterServicesFromAssembly(typeof(HostingExtensions).Assembly));
            registry.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(AssignUserBehavior<,>));
            registry.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(LoggingBehavior<,>));

            registry
                .AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            registry
                .ConfigureApplicationCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                    options.Events.OnRedirectToAccessDenied =
                        options.Events.OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        };
                });

            registry.AddAuthorization();

            registry.Configure<OpenAIOptions>(
                builder.Configuration.GetSection("OpenAI"));

            if (builder.Environment.IsDevelopment())
                registry
                    .AddEndpointsApiExplorer()
                    .AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v1", new OpenApiInfo
                        {
                            Version = "v1",
                            Title = "Lister API"
                        });
                    });
        });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();

            SeedData.EnsureSeedData(app);
        }
        else
        {
            app.UseExceptionHandler();
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}