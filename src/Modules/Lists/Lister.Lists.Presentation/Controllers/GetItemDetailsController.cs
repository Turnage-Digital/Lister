using Lister.Lists.Application.Endpoints.Queries.GetItemDetails;
using Lister.Lists.ReadOnly.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Presentation.Controllers;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetItemDetailsController(IMediator mediator) : Controller
{
    [HttpGet("{listId:guid}/items/{itemId:int}")]
    [ProducesResponseType(typeof(ItemDetailsDto), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        Guid listId,
        int itemId,
        CancellationToken cancellationToken
    )
    {
        var query = new GetItemDetailsQuery(listId, itemId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}