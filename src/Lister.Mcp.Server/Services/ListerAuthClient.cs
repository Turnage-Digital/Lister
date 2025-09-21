using System.Text;
using System.Text.Json;
using Serilog;

namespace Lister.Mcp.Server.Services;

public class ListerAuthClient
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ListerAuthClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        var baseUrl = configuration["ListerApi:BaseUrl"] ?? "http://localhost:5000";
        _httpClient.BaseAddress = new Uri(baseUrl);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<string?> GetValidTokenAsync(CancellationToken cancellationToken = default)
    {
        var email = _configuration["ListerApi:Username"];
        var password = _configuration["ListerApi:Password"];

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Log.Error("Username or password not configured for authentication");
            return null;
        }

        Log.Information("Authenticating with Lister API for fresh token");

        string? retval = null;

        var loginRequest = new { email, password };
        var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/identity/login?useCookies=false",
                content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseJson, _jsonOptions);

                if (tokenResponse != null)
                {
                    Log.Information("Successfully authenticated with Lister API");
                    retval = tokenResponse.AccessToken;
                }
            }

            Log.Error("Failed to authenticate with Lister API with {Response}",
                new { statusCode = response.StatusCode });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during authentication");
        }

        return retval;
    }
}

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
}