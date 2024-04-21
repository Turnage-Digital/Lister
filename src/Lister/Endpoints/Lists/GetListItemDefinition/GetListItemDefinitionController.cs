using Lister.Core.SqlDB.Views;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Lists.GetListItemDefinition;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListItemDefinitionController(IMediator mediator) : Controller
{
    [HttpGet("{listId}/itemDefinition")]
    [ProducesResponseType(typeof(ListItemDefinitionView), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Get(string listId)
    {
        GetListItemDefinitionQuery<ListItemDefinitionView> query = new(listId);
        var result = await mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }
}