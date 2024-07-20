using Lister.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lister.Controllers.Lists;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class DeleteListController(IMediator mediator) : ControllerBase
{
    [HttpDelete("{listId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(string listId)
    {
        var command = new DeleteListCommand
        {
            ListId = listId
        };
        await mediator.Send(command);
        return NoContent();
    }
}