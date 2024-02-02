using System.Security.Claims;
using Lister.Core.SqlDB.Views;
using Lister.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.CreateList;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class CreateListController : Controller
{
    private readonly IMediator _mediator;

    public CreateListController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ListItemDefinitionView), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CreateListCommand<ListItemDefinitionView> command)
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var userId = identity.GetUserId();
        command.CreatedBy = userId;
        var result = await _mediator.Send(command);
        return Created($"/{result.Id}", result);
    }
}