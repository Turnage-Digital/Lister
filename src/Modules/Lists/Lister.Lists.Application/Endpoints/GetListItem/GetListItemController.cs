using Lister.Lists.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.GetListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListItemController(IMediator mediator) : Controller
{
    [HttpGet("{listId}/items/{itemId:int}")]
    [ProducesResponseType(typeof(Item), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        string listId,
        int itemId,
        CancellationToken cancellationToken
    )
    {
        GetListItemQuery query = new(listId, itemId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}