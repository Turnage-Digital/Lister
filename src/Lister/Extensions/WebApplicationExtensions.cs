namespace Lister.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApp(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            SeedData.EnsureSeedData(app);
        }

        app.MapListsApi();
        app.MapUserApi();
        return app;
    }
}