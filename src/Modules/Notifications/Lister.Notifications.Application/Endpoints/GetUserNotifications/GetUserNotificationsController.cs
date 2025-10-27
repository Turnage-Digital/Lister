using Lister.Notifications.ReadOnly.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Notifications.Application.Endpoints.GetUserNotifications;

[ApiController]
[Authorize]
[Tags("Notifications")]
[Route("api/notifications")]
public class GetUserNotificationsController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(NotificationListPageDto), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] DateTime? since,
        [FromQuery] bool? unread,
        [FromQuery] Guid? listId,
        [FromQuery] int pageSize = 20,
        [FromQuery] int page = 0,
        CancellationToken cancellationToken = default
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var query = new GetUserNotificationsQuery
        {
            Since = since,
            Unread = unread,
            ListId = listId,
            PageSize = pageSize,
            Page = page
        };

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
