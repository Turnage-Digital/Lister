using Lister.Lists.Domain.Views;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.CreateList;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class CreateListController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ListItemDefinition), Status201Created)]
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

        CreateListCommand command = new(request.Name, request.Statuses, request.Columns);
        var result = await mediator.Send(command, cancellationToken);
        return Created($"/{result.Id}", result);
    }
}