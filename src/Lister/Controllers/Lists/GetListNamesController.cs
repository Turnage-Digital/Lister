using Lister.Application.Queries;
using Lister.Core.SqlDB.Views;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Controllers.Lists;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListNamesController(IMediator mediator) : Controller
{
    [HttpGet("names")]
    [ProducesResponseType(typeof(ListNameView[]), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        GetListNamesQuery<ListNameView> query = new();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}