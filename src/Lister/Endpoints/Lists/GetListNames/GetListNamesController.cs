using Lister.Core.SqlDB.Views;
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
        GetListNamesQuery<ListNameView> query = new();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}