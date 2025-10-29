using Lister.Lists.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.UpdateList;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId:guid}")]
public class UpdateListController(IMediator mediator) : Controller
{
    [HttpPut]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status409Conflict)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PutAsync(
        [FromRoute] Guid listId,
        [FromBody] UpdateListRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new UpdateListCommand(listId, request.Columns, request.Statuses, request.Transitions);

        try
        {
            await mediator.Send(command, cancellationToken);
            return Ok();
        }
        catch (ListMigrationRequiredException ex)
        {
            return Conflict(new
            {
                message = ex.Message,
                reasons = ex.Reasons,
                plan = ex.Plan
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}