using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lister.Endpoints.Users.SignIn;

public record SignInRequest
{
    [Required]
    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [Required]
    [JsonProperty("password")]
    public string Password { get; set; } = null!;
}