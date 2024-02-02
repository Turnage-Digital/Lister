using System.Security.Claims;
using Lister.Core.ValueObjects;
using Lister.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.GetListItemsByListId;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListItemsByListIdController : Controller
{
    private readonly IMediator _mediator;

    public GetListItemsByListIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}/items")]
    [ProducesResponseType(typeof(Item[]), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Get(
        string id,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortColumn,
        [FromQuery] string? sortOrder
    )
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var userId = identity.GetUserId();
        GetListItemsByListIdQuery query = new(userId, id, page, pageSize, sortColumn, sortOrder);
        var result = await _mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }
}