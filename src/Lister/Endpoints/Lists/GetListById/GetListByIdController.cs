using System.Security.Claims;
using Lister.Core.SqlDB.Views;
using Lister.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.GetListById;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListByIdController : Controller
{
    private readonly IMediator _mediator;

    public GetListByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ListView), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Get(
        string id,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortColumn,
        [FromQuery] string? sortOrder
    )
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var userId = identity.GetUserId();
        GetListByIdQuery<ListView> query = new(userId, id, pageNumber, pageSize, sortColumn, sortOrder);
        var result = await _mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }
}