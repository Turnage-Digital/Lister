using Lister.Lists.ReadOnly.Dtos;
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
    [HttpGet("{listId:guid}/items")]
    [ProducesResponseType(typeof(PagedListDto), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        Guid listId,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? field,
        [FromQuery] string? sort,
        CancellationToken cancellationToken
    )
    {
        var query = new GetPagedListQuery(listId, page, pageSize, field, sort);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
