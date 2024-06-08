using System.ComponentModel.DataAnnotations;
using MediatR;
using Newtonsoft.Json;

namespace Lister.Application.Commands;

public class DeleteListCommand : RequestBase<Unit>
{
    [JsonProperty("listId")]
    [Required]
    public string ListId { get; set; } = null!;
}