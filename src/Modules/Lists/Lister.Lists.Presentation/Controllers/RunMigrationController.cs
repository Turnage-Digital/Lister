using Lister.Lists.Application.Endpoints.Commands.Migrations.RunMigration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Presentation.Controllers;

[ApiController]
[Authorize]
[Tags("List Migrations")]
[Route("api/lists/{listId:guid}/migrations")]
public class RunMigrationController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status401Unauthorized)]
    public async Task<IActionResult> PostAsync(
        [FromRoute] Guid listId,
        [FromBody] RunMigrationRequest? request,
        CancellationToken cancellationToken
    )
    {
        if (request is null)
        {
            return BadRequest(new { message = "A migration request body is required." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new RunMigrationCommand(listId, request.Plan, request.Mode);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}