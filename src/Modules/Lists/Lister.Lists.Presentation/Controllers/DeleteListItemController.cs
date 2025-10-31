using Lister.Lists.Application.Endpoints.Commands.DeleteListItem;
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
public class DeleteListItemController(IMediator mediator) : ControllerBase
{
    [HttpDelete("{listId:guid}/items/{itemId:int}")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(
        Guid listId,
        int itemId,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteListItemCommand(listId, itemId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}