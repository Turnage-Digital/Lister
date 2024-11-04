using Lister.Lists.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.CreateListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class CreateListItemController(IMediator mediator) : Controller
{
    [HttpPost("{listId}/items")]
    [ProducesResponseType(typeof(Item), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        string listId,
        [FromBody] CreateListItemRequest request,
        CancellationToken cancellationToken
    )
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest(ModelState);
        }

        CreateListItemCommand command = new(listId, request.Bag);
        var result = await mediator.Send(command, cancellationToken);
        return Created($"/{result.ListId}/items/{result.Id}", result);
    }
}