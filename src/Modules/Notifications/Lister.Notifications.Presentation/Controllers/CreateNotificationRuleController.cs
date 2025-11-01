using Lister.Notifications.Application.Commands.CreateNotificationRule;
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
[Route("api/notifications/rules")]
public class CreateNotificationRuleController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(NotificationRuleDto), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        [FromBody] CreateNotificationRuleRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        CreateNotificationRuleCommand command = new(
            request.ListId,
            request.Trigger,
            request.Channels,
            request.Schedule,
            request.TemplateId,
            request.IsActive);

        var result = await mediator.Send(command, cancellationToken);
        return Created($"/api/notifications/rules/{result.Id}", result);
    }
}