using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Controllers.Users;

[ApiController]
[AllowAnonymous]
[Tags("Users")]
[Route("api/users")]
public class SignInController(SignInManager<IdentityUser> signInManager) : Controller
{
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(SignInResponse), Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status500InternalServerError)]
    public async Task<IActionResult> Post(SignInRequest input)
    {
        var result = await signInManager.PasswordSignInAsync(input.Username, input.Password,
            true, false);
        return Json(new SignInResponse { Succeeded = result.Succeeded });
    }
}

public record SignInRequest
{
    [Required]
    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [Required]
    [JsonProperty("password")]
    public string Password { get; set; } = null!;
}

public record SignInResponse
{
    [JsonProperty("succeeded")]
    public bool Succeeded { get; set; }
}