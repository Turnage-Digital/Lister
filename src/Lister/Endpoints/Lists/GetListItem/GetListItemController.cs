using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.GetListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListItemController(IMediator mediator) : Controller
{
    [HttpGet("{listId}/items/{itemId}")]
    [ProducesResponseType(typeof(Item), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Get(string listId, string itemId)
    {
        GetListItemQuery query = new(listId, itemId);
        var result = await mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }
}