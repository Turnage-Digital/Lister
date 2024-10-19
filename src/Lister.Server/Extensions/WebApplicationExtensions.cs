namespace Lister.Server.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApplication(this WebApplication app)
    {
        app.MapGroup("/api")
            .MapListsApi();

        app.MapIdentityApi();
        return app;
    }
}