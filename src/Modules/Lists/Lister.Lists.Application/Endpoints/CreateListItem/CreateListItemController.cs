using Lister.Lists.ReadOnly.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.CreateListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class CreateListItemController(IMediator mediator) : Controller
{
    [HttpPost("{listId:guid}/items")]
    [ProducesResponseType(typeof(ItemDetailsDto), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        Guid listId,
        [FromBody] CreateListItemRequest request,
        CancellationToken cancellationToken
    )
    {
        if (ModelState.IsValid is false)
        {
            return BadRequest(ModelState);
        }

        var command = new CreateListItemCommand(listId, request.Bag);
        var result = await mediator.Send(command, cancellationToken);
        return Created($"/{result.ListId}/items/{result.Id}", result);
    }
}
