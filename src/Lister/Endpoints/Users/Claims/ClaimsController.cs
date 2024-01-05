using Lister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lister.Endpoints.Users.Claims;

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