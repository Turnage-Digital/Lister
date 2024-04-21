using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.CreateListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class CreateListItemController(IMediator mediator) : Controller
{
    [HttpPost("{listId}/items/create")]
    [ProducesResponseType(typeof(Item), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post(string listId, [FromBody] CreateListItemCommand command)
    {
        command.ListId = listId;
        var result = await mediator.Send(command);
        return Created($"/{listId}/items/{result.Id}", result);
    }
}