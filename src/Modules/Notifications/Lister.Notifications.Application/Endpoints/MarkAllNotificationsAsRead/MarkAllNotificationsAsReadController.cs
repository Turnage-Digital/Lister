using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Notifications.Application.Endpoints.MarkAllNotificationsAsRead;

[ApiController]
[Authorize]
[Tags("Notifications")]
[Route("api/notifications/readAll")]
public class MarkAllNotificationsAsReadController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        [FromBody] Request request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new MarkAllNotificationsAsReadCommand { Before = request.Before };
        await mediator.Send(command, cancellationToken);
        return Ok();
    }

    public class Request
    {
        public DateTime? Before { get; set; }
    }
}