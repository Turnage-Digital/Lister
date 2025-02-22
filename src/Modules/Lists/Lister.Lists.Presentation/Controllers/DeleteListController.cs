using Lister.Lists.Application.Commands.DeleteList;
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
public class DeleteListController(IMediator mediator) : ControllerBase
{
    [HttpDelete("{listId}")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(
        string listId,
        CancellationToken cancellationToken
    )
    {
        DeleteListCommand command = new(listId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}