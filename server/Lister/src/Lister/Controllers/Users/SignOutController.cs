using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Controllers.Users;

[ApiController]
[Authorize]
[Tags("Users")]
[Route("api/users")]
public class SignOutController(SignInManager<IdentityUser> signInManager) : Controller
{
    [HttpPost("sign-out")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}