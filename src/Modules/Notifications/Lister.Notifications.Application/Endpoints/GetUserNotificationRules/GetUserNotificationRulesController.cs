using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Notifications.Application.Endpoints.GetUserNotificationRules;

[ApiController]
[Authorize]
[Tags("Notifications")]
[Route("api/notifications/rules")]
public class GetUserNotificationRulesController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(GetUserNotificationRulesResponse), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] string userId,
        [FromQuery] Guid? listId = null,
        CancellationToken cancellationToken = default
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        GetUserNotificationRulesQuery query = new(userId, listId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}