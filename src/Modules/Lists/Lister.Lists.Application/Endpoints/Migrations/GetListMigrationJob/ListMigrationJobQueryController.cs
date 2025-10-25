using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.Migrations.GetListMigrationJob;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId:guid}/migrations/{jobId:guid}")]
public class ListMigrationJobQueryController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] Guid listId,
        [FromRoute] Guid jobId,
        CancellationToken ct
    )
    {
        var query = new GetListMigrationJobQuery(listId, jobId);
        var job = await mediator.Send(query, ct);
        if (job is null)
        {
            return NotFound();
        }

        return Ok(job);
    }
}