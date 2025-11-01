using Lister.Notifications.Application.Queries.GetUnreadNotificationCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Notifications.Presentation.Controllers;

[ApiController]
[Authorize]
[Tags("Notifications")]
[Route("api/notifications/unreadCount")]
public class GetUnreadNotificationCountController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(int), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] Guid? listId = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var query = new GetUnreadNotificationCountQuery { ListId = listId };
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}