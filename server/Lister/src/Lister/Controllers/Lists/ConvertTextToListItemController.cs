using Lister.Application.Commands;
using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Controllers.Lists;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class ConvertTextToListItemController(IMediator mediator) : Controller
{
    [HttpPost("convert-text-to-list-item")]
    [ProducesResponseType(typeof(Item), 200)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] ConvertTextToListItemCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}