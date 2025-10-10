using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.Migrations;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId:guid}/migrations")]
public class MigrationsController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        [FromRoute] Guid listId,
        [FromBody] RunMigrationRequest request,
        CancellationToken ct
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var mode = string.Equals(request.Mode, "execute", StringComparison.OrdinalIgnoreCase)
            ? MigrationMode.Execute
            : MigrationMode.DryRun;

        var command = new RunMigrationCommand(listId, request.Plan, mode)
        {
            UserId = HttpContext.User.Identity?.Name
        };

        var result = await mediator.Send(command, ct);
        return Ok(result);
    }

    public record RunMigrationRequest(MigrationPlan Plan, string Mode);
}