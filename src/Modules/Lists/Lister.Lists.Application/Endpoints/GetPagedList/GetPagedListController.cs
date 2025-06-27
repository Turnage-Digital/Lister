using Lister.Lists.Domain.Views;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.GetPagedList;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/")]
public class GetPagedListController(IMediator mediator) : Controller
{
    [HttpGet("{listId}/items")]
    [ProducesResponseType(typeof(PagedList), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        string listId,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? field,
        [FromQuery] string? sort,
        CancellationToken cancellationToken
    )
    {
        GetPagedListQuery query = new(listId, page, pageSize, field, sort);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}