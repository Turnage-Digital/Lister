using Microsoft.AspNetCore.Identity;

namespace Lister.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApplication(this WebApplication app)
    {
        app.MapListsApi();
        app.MapIdentity();
        return app;
    }
}