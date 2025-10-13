using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Lists.Application.Endpoints.GetItemHistory;

[ApiController]
[Authorize]
[Tags("Lists")]
[Route("api/lists/{listId:guid}/items/{itemId:int}/history")]
public class GetItemHistoryController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(HistoryPage<ItemHistoryType>), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        Guid listId,
        int itemId,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        if (page < 0 || pageSize <= 0)
        {
            return BadRequest("Invalid pagination arguments.");
        }

        var query = new GetItemHistoryQuery(listId, itemId, page, pageSize)
        {
            UserId = HttpContext.User?.Identity?.Name ?? string.Empty
        };

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}