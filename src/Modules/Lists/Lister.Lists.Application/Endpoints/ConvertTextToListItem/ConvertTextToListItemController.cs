using Lister.Lists.Domain.Views;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.ConvertTextToListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class ConvertTextToListItemController(IMediator mediator) : Controller
{
    [HttpPost("{listId:guid}/items/convert-text-to-list-item")]
    [ProducesResponseType(typeof(ListItem), 200)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        Guid listId,
        [FromBody] ConvertTextToListItemRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ConvertTextToListItemCommand command = new(listId, request.Text);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}