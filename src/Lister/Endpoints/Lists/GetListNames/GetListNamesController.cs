using System.Security.Claims;
using Lister.Core.SqlDB.Views;
using Lister.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.GetListNames;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListNamesController : Controller
{
    private readonly IMediator _mediator;

    public GetListNamesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("names")]
    [ProducesResponseType(typeof(ListNameView[]), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var userId = identity.GetUserId();
        GetListNamesQuery<ListNameView> query = new(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}