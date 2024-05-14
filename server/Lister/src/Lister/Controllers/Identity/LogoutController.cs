using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Controllers.Identity;

[ApiController]
[Authorize]
[Tags("Identity")]
[Route("identity")]
public class LogoutController(SignInManager<IdentityUser> signInManager) : Controller
{
    [HttpPost("logout")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}