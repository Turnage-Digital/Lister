using System.Security.Claims;
using Lister.Core.ValueObjects;
using Lister.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.CreateListItem;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class CreateListItemController : Controller
{
    private readonly IMediator _mediator;

    public CreateListItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{id}/items/create")]
    [ProducesResponseType(typeof(Item), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post(string id, [FromBody] CreateListItemCommand command)
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var userId = identity.GetUserId();
        command.CreatedBy = userId;
        command.ListId = id;
        var result = await _mediator.Send(command);
        return Created($"/{id}/items/{result.Id}", result);
    }
}