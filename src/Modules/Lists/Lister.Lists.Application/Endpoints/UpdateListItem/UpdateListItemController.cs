using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.UpdateListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId}/items/{itemId}")]
public class UpdateListItemController(IMediator mediator) : Controller
{
    [HttpPut]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PutAsync(
        [FromRoute] Guid listId,
        [FromRoute] int itemId,
        [FromBody] object bag,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new UpdateListItemCommand(listId, itemId, bag);
        await mediator.Send(command, cancellationToken);
        return Ok();
    }
}
