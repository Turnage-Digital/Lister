using Lister.Lists.ReadOnly.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.GetListItemDefinition;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetListItemDefinitionController(IMediator mediator) : Controller
{
    [HttpGet("{listId:guid}/itemDefinition")]
    [ProducesResponseType(typeof(ListItemDefinitionDto), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        Guid listId,
        CancellationToken cancellationToken
    )
    {
        var query = new GetListItemDefinitionQuery(listId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
