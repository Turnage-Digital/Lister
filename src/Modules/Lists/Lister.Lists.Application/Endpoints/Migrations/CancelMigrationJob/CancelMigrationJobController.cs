using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.Migrations.CancelMigrationJob;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId:guid}/migrations/{jobId:guid}/cancel")]
public class CancelMigrationJobController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(Status202Accepted)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    public async Task<IActionResult> PostAsync(
        [FromRoute] Guid listId,
        [FromRoute] Guid jobId,
        CancellationToken ct
    )
    {
        var command = new CancelMigrationJobCommand(listId, jobId);
        var requested = await mediator.Send(command, ct);
        if (!requested)
        {
            return NotFound();
        }

        return Accepted(new { jobId, status = "CancelRequested" });
    }
}