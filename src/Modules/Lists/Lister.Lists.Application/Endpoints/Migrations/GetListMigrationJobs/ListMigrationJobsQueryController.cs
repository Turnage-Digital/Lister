using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.Migrations.GetListMigrationJobs;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId:guid}/migrations")]
public class ListMigrationJobsQueryController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] Guid listId,
        CancellationToken ct
    )
    {
        var query = new GetListMigrationJobsQuery(listId);
        var jobs = await mediator.Send(query, ct);
        return Ok(jobs);
    }
}