using Lister.Application;
using Lister.Application.SqlDB;
using Lister.Behaviors;
using Lister.Core.SqlDB;
using Lister.Domain;
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
        var seqUrl = builder.Configuration["SeqUrl"]!;
        builder.Host.UseSerilog((_, config) => config
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level} {SourceContext}]{NewLine}{Message:lj}{NewLine}{NewLine}")
            .WriteTo.Seq(seqUrl)
            .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
            .Enrich.FromLogContext());

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();
        
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        builder.Services.AddCore(connectionString);
        builder.Services.AddDomain();
        
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(IDomainMarker).Assembly);
            config.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly);
        });
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(AssignUserBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        builder.Services
            .AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

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

        builder.Services.Configure<OpenAIOptions>(
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
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();

            // SeedData.EnsureSeedData(app);
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

        app.MapGroup("/identity")
            .WithTags("Identity")
            .MapIdentityApi<IdentityUser>();

        return app;
    }
}