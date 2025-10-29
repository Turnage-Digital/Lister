using System.ComponentModel.DataAnnotations;
using Lister.Lists.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.Migrations;

[ApiController]
[Authorize]
[Tags("List Migrations")]
[Route("api/lists/{listId:guid}/migrations")]
public class MigrationsController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status401Unauthorized)]
    public async Task<IActionResult> PostAsync(
        [FromRoute] Guid listId,
        [FromBody] RunMigrationRequest request,
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

        if (request.Plan is null)
        {
            return BadRequest(new { message = "A migration plan is required." });
        }

        var command = new RunMigrationCommand(listId, request.Plan, request.Mode);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{correlationId:guid}")]
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

public class RunMigrationRequest
{
    [Required]
    public MigrationPlan Plan { get; init; } = null!;

    public MigrationMode Mode { get; init; } = MigrationMode.Execute;
}