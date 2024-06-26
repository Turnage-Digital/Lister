using Newtonsoft.Json;

namespace Lister.Endpoints.Users.Claims;

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