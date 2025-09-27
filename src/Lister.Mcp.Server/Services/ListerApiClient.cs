using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lister.Lists.Application.Endpoints.CreateList;
using Lister.Lists.Application.Endpoints.CreateListItem;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Domain.Views;
using Lister.Notifications.Domain.ValueObjects;
using Lister.Notifications.Domain.Views;

namespace Lister.Mcp.Server.Services;

public class ListerApiClient
{
    private readonly ListerAuthClient _authService;
    private readonly HttpClient _httpClient;
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
        HttpMethod method,
        string requestUri,
        CancellationToken cancellationToken
    )
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
        using var request =
            await CreateAuthenticatedRequestAsync(HttpMethod.Get, "/api/lists/names", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ListName[]>(json, _jsonOptions) ?? [];
    }

    public async Task<ListItemDefinition> GetListSchemaAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"/api/lists/{listId}/itemDefinition",
            cancellationToken);
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
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get,
            $"/api/lists/{listId}/items?{queryString}", cancellationToken);
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
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"/api/lists/{listId}/items/{itemId}",
            cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ItemDetails>(json, _jsonOptions) ?? new ItemDetails();
    }

    public async Task UpdateListItemAsync(
        Guid listId,
        int itemId,
        Dictionary<string, object?> bag,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(bag, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put,
            $"/api/lists/{listId}/items/{itemId}", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<StatusTransition[]> GetStatusTransitionsAsync(
        Guid listId,
        CancellationToken cancellationToken = default
    )
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get,
            $"/api/lists/{listId}/statusTransitions", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<StatusTransition[]>(json, _jsonOptions) ?? [];
    }

    public async Task SetStatusTransitionsAsync(
        Guid listId,
        StatusTransition[] transitions,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(transitions, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put,
            $"/api/lists/{listId}/statusTransitions", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    // Notifications
    public async Task<NotificationListPage> GetUserNotificationsAsync(
        DateTime? since = null,
        bool? unread = null,
        Guid? listId = null,
        int pageSize = 20,
        int page = 0,
        CancellationToken cancellationToken = default
    )
    {
        var qp = new List<string> { $"pageSize={pageSize}", $"page={page}" };
        if (since is not null)
        {
            qp.Add($"since={Uri.EscapeDataString(since.Value.ToString("O"))}");
        }

        if (unread is not null)
        {
            qp.Add($"unread={(unread.Value ? "true" : "false")}");
        }

        if (listId is not null)
        {
            qp.Add($"listId={listId}");
        }

        var uri = "/api/notifications" + (qp.Count > 0 ? "?" + string.Join("&", qp) : string.Empty);
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, uri, cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<NotificationListPage>(json, _jsonOptions) ?? new NotificationListPage();
    }

    public async Task<NotificationDetails?> GetNotificationDetailsAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default
    )
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get,
            $"/api/notifications/{notificationId}", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<NotificationDetails>(json, _jsonOptions);
    }

    public async Task<int> GetUnreadNotificationCountAsync(
        Guid? listId = null,
        CancellationToken cancellationToken = default
    )
    {
        var uri = "/api/notifications/unreadCount" + (listId is not null ? $"?listId={listId}" : "");
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, uri, cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return int.TryParse(json, out var n) ? n : 0;
    }

    public async Task MarkNotificationAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post,
            $"/api/notifications/{notificationId}/read", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task MarkAllNotificationsAsReadAsync(
        DateTime? before = null,
        CancellationToken cancellationToken = default
    )
    {
        var payload = new { before = before?.ToString("O") };
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post,
            "/api/notifications/readAll", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<NotificationRule> CreateNotificationRuleAsync(
        Guid listId,
        NotificationTrigger trigger,
        NotificationChannel[] channels,
        NotificationSchedule schedule,
        string? templateId,
        CancellationToken cancellationToken = default
    )
    {
        var body = new
        {
            listId,
            trigger,
            channels,
            schedule,
            templateId
        };
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post,
            "/api/notifications/rules", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<NotificationRule>(responseJson, _jsonOptions) ?? new NotificationRule();
    }

    public async Task UpdateNotificationRuleAsync(
        Guid ruleId,
        NotificationTrigger trigger,
        NotificationChannel[] channels,
        NotificationSchedule schedule,
        string? templateId,
        CancellationToken cancellationToken = default
    )
    {
        var body = new { trigger, channels, schedule, templateId };
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put,
            $"/api/notifications/rules/{ruleId}", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteNotificationRuleAsync(
        Guid ruleId,
        CancellationToken cancellationToken = default
    )
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Delete,
            $"/api/notifications/rules/{ruleId}", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    // SSE Change Feed
    public async Task<string[]> ReadChangeFeedAsync(
        int maxEvents = 10,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    )
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (timeout is not null)
        {
            cts.CancelAfter(timeout.Value);
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/changes/stream");
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/event-stream"));
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cts.Token);
        using var reader = new StreamReader(stream);
        var events = new List<string>();
        while (!reader.EndOfStream && events.Count < maxEvents && !cts.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cts.Token);
            if (line is null) break;
            if (line.StartsWith("data: "))
            {
                var json = line.Substring("data: ".Length);
                events.Add(json);
            }
        }

        return events.ToArray();
    }

    public async Task<ListItem> CreateListItemAsync(
        Guid listId,
        Dictionary<string, object?> data,
        CancellationToken cancellationToken = default
    )
    {
        var requestData = new CreateListItemRequest { Bag = data };
        var json = JsonSerializer.Serialize(requestData, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var request =
            await CreateAuthenticatedRequestAsync(HttpMethod.Post, $"/api/lists/{listId}/items", cancellationToken);
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
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, "/api/lists", cancellationToken);
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ListName>(responseJson, _jsonOptions) ?? new ListName();
    }

    public async Task DeleteListItemAsync(Guid listId, int itemId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Delete,
            $"/api/lists/{listId}/items/{itemId}", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteListAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        using var request =
            await CreateAuthenticatedRequestAsync(HttpMethod.Delete, $"/api/lists/{listId}", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> GetSwaggerJsonAsync(CancellationToken cancellationToken = default)
    {
        using var request =
            await CreateAuthenticatedRequestAsync(HttpMethod.Get, "/swagger/v1/swagger.json", cancellationToken);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
