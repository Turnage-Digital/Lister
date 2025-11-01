using Lister.Notifications.Application.Commands.MarkNotificationAsRead;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Notifications.Presentation.Controllers;

[ApiController]
[Authorize]
[Tags("Notifications")]
[Route("api/notifications/{notificationId}/read")]
public class MarkNotificationAsReadController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        [FromRoute] Guid notificationId,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new MarkNotificationAsReadCommand { NotificationId = notificationId };
        await mediator.Send(command, cancellationToken);
        return Ok();
    }
}