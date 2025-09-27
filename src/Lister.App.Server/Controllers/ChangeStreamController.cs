using Lister.App.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lister.App.Server.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/changes/stream")]
public class ChangeStreamController(ChangeFeed feed) : ControllerBase
{
    [HttpGet]
    public async Task Stream(CancellationToken cancellationToken)
    {
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("X-Accel-Buffering", "no");

        var reader = feed.Reader;
        await foreach (var json in reader.ReadAllAsync(cancellationToken))
        {
            var line = $"data: {json}\n\n";
            await Response.WriteAsync(line, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}