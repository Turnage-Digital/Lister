using Lister.Core.SqlDB.Views;
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
        GetListItemDefinitionByIdQuery<ListItemDefinitionView> query = new(id);
        var result = await _mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }
}