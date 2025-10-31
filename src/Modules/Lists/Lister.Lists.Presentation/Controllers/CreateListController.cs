using Lister.Lists.Application.Commands.CreateList;
using Lister.Lists.ReadOnly.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Presentation.Controllers;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class CreateListController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ListItemDefinitionDto), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        [FromBody] CreateListRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        CreateListCommand command = new(request.Name, request.Statuses, request.Columns, request.Transitions);
        var result = await mediator.Send(command, cancellationToken);
        return Created($"/{result.Id}", result);
    }
}