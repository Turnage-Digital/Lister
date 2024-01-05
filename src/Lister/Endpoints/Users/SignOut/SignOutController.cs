using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Users.SignOut;

[ApiController]
[Authorize]
[Tags("Users")]
[Route("api/users")]
public class SignOutController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public SignOutController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("sign-out")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}