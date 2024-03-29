using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.ConvertTextToListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class ConvertTextToListItemController : Controller
{
    private readonly IMediator _mediator;

    public ConvertTextToListItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("convert-text-to-list-item")]
    [ProducesResponseType(typeof(Item), 200)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] ConvertTextToListItemCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}