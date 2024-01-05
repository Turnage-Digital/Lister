using Newtonsoft.Json;

namespace Lister.Endpoints.Users.SignIn;

public record SignInResponse
{
    [JsonProperty("succeeded")]
    public bool Succeeded { get; set; }
}