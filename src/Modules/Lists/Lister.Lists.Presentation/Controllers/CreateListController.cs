using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Lister.Lists.Application.Commands.CreateList;
using Lister.Lists.Domain.ValueObjects;
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
public class CreateListController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(ListItemDefinition), Status201Created)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> PostAsync(
        [FromBody] CreateListRequest request,
        CancellationToken cancellationToken
    )
    {
        if (ModelState.IsValid is false)
            return BadRequest(ModelState);

        CreateListCommand command = new(request.Name, request.Statuses, request.Columns);
        var result = await mediator.Send(command, cancellationToken);
        return Created($"/{result.Id}", result);
    }

    public record CreateListRequest
    {
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; } = null!;

        [JsonPropertyName("statuses")]
        [Required]
        public Status[] Statuses { get; set; } = null!;

        [JsonPropertyName("columns")]
        [Required]
        public Column[] Columns { get; set; } = null!;
    }
}