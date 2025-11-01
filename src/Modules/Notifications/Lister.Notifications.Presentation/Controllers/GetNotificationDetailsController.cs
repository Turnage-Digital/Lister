using Lister.Notifications.Application.Queries.GetNotificationDetails;
using Lister.Notifications.ReadOnly.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Notifications.Presentation.Controllers;

[ApiController]
[Authorize]
[Tags("Notifications")]
[Route("api/notifications/{notificationId}")]
public class GetNotificationDetailsController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(NotificationDetailsDto), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] Guid notificationId,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var query = new GetNotificationDetailsQuery { NotificationId = notificationId };
        var result = await mediator.Send(query, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}