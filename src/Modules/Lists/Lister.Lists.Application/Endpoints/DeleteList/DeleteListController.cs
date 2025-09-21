using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.DeleteList;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class DeleteListController(IMediator mediator) : ControllerBase
{
    [HttpDelete("{listId:guid}")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(
        Guid listId,
        CancellationToken cancellationToken
    )
    {
        DeleteListCommand command = new(listId);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}