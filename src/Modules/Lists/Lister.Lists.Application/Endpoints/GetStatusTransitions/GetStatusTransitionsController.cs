using Lister.Lists.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.GetStatusTransitions;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId}/statusTransitions")]
public class GetStatusTransitionsController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(StatusTransition[]), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] Guid listId,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await mediator.Send(new GetStatusTransitionsQuery(listId), cancellationToken);
        return Ok(result);
    }
}
