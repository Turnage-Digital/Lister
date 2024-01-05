using Lister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Endpoints.Users.SignIn;

[ApiController]
[AllowAnonymous]
[Tags("Users")]
[Route("api/users")]
public class SignInController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public SignInController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(SignInResponse), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post(SignInRequest input)
    {
        var result = await _signInManager.PasswordSignInAsync(input.Username, input.Password,
            true, false);
        return Json(new SignInResponse { Succeeded = result.Succeeded });
    }
}