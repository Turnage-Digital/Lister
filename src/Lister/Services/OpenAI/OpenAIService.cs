using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Lister.Services.OpenAi;

public class OpenAIService : IOpenAIService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public OpenAIService(HttpClient httpClient, IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        _apiKey = options.Value.ApiKey;
    }

    public async Task<string> CreateCompletion(string model, string[] messages)
    {
        var requestContent = new
        {
            model,
            messages
        };

        var jsonContent = JsonConvert.SerializeObject(requestContent);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync("engines/davinci/completions", httpContent);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"OpenAI API request failed with status code {response.StatusCode}");
        }

        var retval = await response.Content.ReadAsStringAsync();
        return retval;
    }
}