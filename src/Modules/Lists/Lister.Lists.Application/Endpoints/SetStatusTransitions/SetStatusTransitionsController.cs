using Lister.Lists.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.SetStatusTransitions;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId}/statusTransitions")]
public class SetStatusTransitionsController(IMediator mediator) : Controller
{
    [HttpPut]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PutAsync(
        [FromRoute] Guid listId,
        [FromBody] StatusTransition[] transitions,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await mediator.Send(new SetStatusTransitionsCommand(listId, transitions), cancellationToken);
        return Ok();
    }
}