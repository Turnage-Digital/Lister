using Lister.Notifications.Application.Commands.DeleteNotificationRule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Notifications.Presentation.Controllers;

[ApiController]
[Authorize]
[Tags("Notifications")]
[Route("api/notifications/rules")]
public class DeleteNotificationRuleController(IMediator mediator) : Controller
{
    [HttpDelete("{ruleId}")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] Guid ruleId,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        DeleteNotificationRuleCommand command = new(ruleId);
        await mediator.Send(command, cancellationToken);
        return Ok();
    }
}