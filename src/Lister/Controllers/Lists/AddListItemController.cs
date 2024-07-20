using Lister.Application.Commands;
using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Controllers.Lists;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class AddListItemController(IMediator mediator) : ControllerBase
{
    [HttpPost("{listId}/items/add")]
    [ProducesResponseType(typeof(Item), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post(string listId, [FromBody] AddListItemCommand command)
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest(ModelState);
        }
        
        command.ListId = listId;
        var result = await mediator.Send(command);
        return Created($"/{listId}/items/{result.Id}", result);
    }
}