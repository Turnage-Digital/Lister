using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lister.Endpoints.Lists.CreateListItem;

// retval.MapPost("/{id}/items/create", async (
//         Guid id,
//         CreateListItemCommand command,
//         IMediator mediator,
//         ClaimsPrincipal claimsPrincipal
//     ) =>
//     {
//         var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
//         var userId = identity.GetUserId();
//         command.CreatedBy = userId;
//         command.ListId = id;
//         var result = await mediator.Send(command);
//         return Results.Created($"/{id}/items/{result.Id}", result);
//     })
//     .Produces(Status401Unauthorized)
//     .Produces<Item>(Status201Created)
//     .Produces(Status500InternalServerError);

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

    // [HttpPost("/{id}/items/create")]
}