using Lister.Lists.ReadOnly.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.GetListNames;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListNamesController(IMediator mediator) : Controller
{
    [HttpGet("names")]
    [ProducesResponseType(typeof(ListNameDto[]), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var query = new GetListNamesQuery();
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
