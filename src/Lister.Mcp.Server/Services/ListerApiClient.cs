using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lister.Lists.Application.Endpoints.ConvertTextToListItem;
using Lister.Lists.Application.Endpoints.CreateList;
using Lister.Lists.Application.Endpoints.CreateListItem;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Domain.Views;

namespace Lister.Mcp.Server.Services;

public class ListerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ListerAuthClient _authService;
    private readonly JsonSerializerOptions _jsonOptions;

    public ListerApiClient(HttpClient httpClient, IConfiguration configuration, ListerAuthClient authService)
    {
        _httpClient = httpClient;
        _authService = authService;
        
        var baseUrl = configuration["ListerApi:BaseUrl"] ?? "http://localhost:5000";
        _httpClient.BaseAddress = new Uri(baseUrl);
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    private async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(
        HttpMethod method, string requestUri, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(method, requestUri);
        
        var token = await _authService.GetValidTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        return request;
    }

    public async Task<ListName[]> GetListsAsync(CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, "/api/lists/names", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ListName[]>(json, _jsonOptions) ?? [];
    }

    public async Task<ListItemDefinition> GetListSchemaAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"/api/lists/{listId}/itemDefinition", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ListItemDefinition>(json, _jsonOptions) ?? new ListItemDefinition();
    }

    public async Task<PagedList> GetListItemsAsync(
        Guid listId,
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? sortBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default
    )
    {
        var queryParams = new List<string>
        {
            $"page={page}",
            $"pageSize={pageSize}"
        };

        if (!string.IsNullOrEmpty(searchTerm))
        {
            queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
            queryParams.Add($"ascending={ascending}");
        }

        var queryString = string.Join("&", queryParams);
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"/api/lists/{listId}/items?{queryString}", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<PagedList>(json, _jsonOptions) ?? new PagedList();
    }

    public async Task<ItemDetails> GetItemDetailsAsync(
        Guid listId,
        int itemId,
        CancellationToken cancellationToken = default
    )
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"/api/lists/{listId}/items/{itemId}", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ItemDetails>(json, _jsonOptions) ?? new ItemDetails();
    }

    public async Task<ListItem> CreateListItemAsync(
        Guid listId,
        Dictionary<string, object?> data,
        CancellationToken cancellationToken = default
    )
    {
        var requestData = new CreateListItemRequest { Bag = data };
        var json = JsonSerializer.Serialize(requestData, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, $"/api/lists/{listId}/items", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ListItem>(responseJson, _jsonOptions) ?? new ListItem();
    }

    public async Task<ListItem> SmartCreateItemAsync(
        Guid listId,
        string text,
        CancellationToken cancellationToken = default
    )
    {
        var requestData = new ConvertTextToListItemRequest { Text = text };
        var json = JsonSerializer.Serialize(requestData, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, $"/api/lists/{listId}/items/convert-text-to-list-item", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ListItem>(responseJson, _jsonOptions) ?? new ListItem();
    }

    public async Task<ListName> CreateListAsync(
        string name,
        Column[] columns,
        Status[] statuses,
        CancellationToken cancellationToken = default
    )
    {
        var requestData = new CreateListRequest
        {
            Name = name,
            Columns = columns,
            Statuses = statuses
        };

        var json = JsonSerializer.Serialize(requestData, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, "/api/lists", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ListName>(responseJson, _jsonOptions) ?? new ListName();
    }

    public async Task DeleteListItemAsync(Guid listId, int itemId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Delete, $"/api/lists/{listId}/items/{itemId}", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteListAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Delete, $"/api/lists/{listId}", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}