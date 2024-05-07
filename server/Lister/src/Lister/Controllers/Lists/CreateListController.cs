using Lister.Application.Commands;
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
public class CreateListController(IMediator mediator) : Controller
{
    [HttpPost("create")]
    [ProducesResponseType(typeof(ListItemDefinitionView), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CreateListCommand<ListItemDefinitionView> command)
    {
        var result = await mediator.Send(command);
        return Created($"/{result.Id}", result);
    }
}