using Lister.Notifications.Application.Endpoints.Commands.UpdateNotificationRule;
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
public class UpdateNotificationRuleController(IMediator mediator) : Controller
{
    [HttpPut("{ruleId}")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PutAsync(
        [FromRoute] Guid ruleId,
        [FromBody] UpdateNotificationRuleRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        UpdateNotificationRuleCommand command = new(
            ruleId,
            request.Trigger,
            request.Channels,
            request.Schedule,
            request.TemplateId,
            request.IsActive);

        await mediator.Send(command, cancellationToken);
        return Ok();
    }
}