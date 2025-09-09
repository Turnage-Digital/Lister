using Lister.Mcp.Server.Services;
using Serilog;

namespace Lister.Mcp.Server;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, config) => config
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level} {SourceContext}]{NewLine}{Message:lj}{NewLine}{NewLine}")
            .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
            .Enrich.FromLogContext());

        builder.Services.AddHttpClient<ListerApiClient>();
        builder.Services.AddHttpClient<ListerAuthClient>();

        builder.Services
            .AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.MapMcp();

        return app;
    }
}