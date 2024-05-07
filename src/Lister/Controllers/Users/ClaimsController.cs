using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lister.Controllers.Users;

using static StatusCodes;

[ApiController]
[Authorize]
[Tags("Users")]
[Route("api/users")]
public class ClaimsController : Controller
{
    [HttpGet("claims")]
    [ProducesResponseType(typeof(ClaimsResponse), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public IActionResult Get()
    {
        var claims = User.Claims
            .Select(x => new Claim { Type = x.Type, Value = x.Value })
            .ToArray();
        return Json(new ClaimsResponse { Claims = claims });
    }
}

public record ClaimsResponse
{
    [JsonProperty("claims")]
    public Claim[] Claims { get; set; } = null!;
}

public record Claim
{
    [JsonProperty("type")]
    public string Type { get; set; } = null!;

    [JsonProperty("value")]
    public object Value { get; set; } = null!;
}