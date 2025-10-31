using Lister.Lists.Application.Endpoints.Queries.Migrations.GetMigrationJobStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Presentation.Controllers;

[ApiController]
[Authorize]
[Tags("List Migrations")]
[Route("api/lists/{listId:guid}/migrations/{correlationId:guid}")]
public class GetMigrationJobStatusController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> GetAsync(
        [FromRoute] Guid listId,
        [FromRoute] Guid correlationId,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(new GetMigrationJobStatusQuery(listId, correlationId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}