using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Lists.Application.Commands.ConvertTextToListItem;
using Lister.Lists.Domain.Views;
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
public class ConvertTextToListItemController(IMediator mediator) : Controller
{
    [HttpPost("{listId}/items/convert-text-to-list-item")]
    [ProducesResponseType(typeof(ListItem), 200)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        string listId,
        [FromBody] ConvertTextToListItemRequest request,
        CancellationToken cancellationToken
    )
    {
        if (ModelState.IsValid is false)
            return BadRequest(ModelState);

        ConvertTextToListItemCommand command = new(listId, request.Text);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    public record ConvertTextToListItemRequest
    {
        [JsonPropertyName("text")]
        [Required]
        public string Text { get; set; } = string.Empty;
    }
}