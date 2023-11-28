using Newtonsoft.Json;

namespace Lister.Models;

public record SignInResponse
{
    [JsonProperty("succeeded")]
    public bool Succeeded { get; set; }
}