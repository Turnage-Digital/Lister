using System.Security.Claims;
using Lister.Core.ValueObjects;
using Lister.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.GetListItemById;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListItemByIdController : Controller
{
    private readonly IMediator _mediator;

    public GetListItemByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{listId}/items/{itemId}")]
    [ProducesResponseType(typeof(Item), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Get(string listId, string itemId)
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var userId = identity.GetUserId();
        GetListItemByIdQuery query = new(userId, listId, itemId);
        var result = await _mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }
}