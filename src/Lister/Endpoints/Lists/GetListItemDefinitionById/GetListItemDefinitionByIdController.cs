using System.Security.Claims;
using Lister.Core.SqlDB.Views;
using Lister.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.GetListItemDefinitionById;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListItemDefinitionByIdController : Controller
{
    private readonly IMediator _mediator;

    public GetListItemDefinitionByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}/itemDefinition")]
    [ProducesResponseType(typeof(ListItemDefinitionView), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Get(string id)
    {
        var identity = (ClaimsIdentity)User.Identity!;
        var userId = identity.GetUserId();
        GetListItemDefinitionByIdQuery<ListItemDefinitionView> query = new(userId, id);
        var result = await _mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }
}