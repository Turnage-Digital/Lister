using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lister.Lists.Domain.ValueObjects;
using Lister.Mcp.Server.Services;
using Lister.Notifications.Domain.ValueObjects;
using ModelContextProtocol.Server;
using Serilog;

namespace Lister.Mcp.Server.Tools;

[McpServerToolType]
public class ListerTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    [McpServerTool]
    [Description("Get all available lists in the Lister application")]
    public static async Task<string> ListLists(
        ListerApiClient apiClient,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("ListLists called");
        try
        {
            var lists = await apiClient.GetListsAsync(cancellationToken);
            Log.Information("Successfully retrieved lists with {Result}", new { listCount = lists.Length });

            var result = new
            {
                success = true,
                lists = lists.Select(list => new
                    {
                        id = list.Id,
                        name = list.Name,
                        itemCount = list.Count
                    })
                    .ToArray(),
                totalLists = lists.Length
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve lists");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Get the schema (columns and statuses) for a specific list")]
    public static async Task<string> GetListSchema(
        ListerApiClient apiClient,
        [Description("The ID of the list to get schema for")]
        string listId,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("GetListSchema called with {Request}", new { listId });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            var schema = await apiClient.GetListSchemaAsync(guid, cancellationToken);
            Log.Information("Successfully retrieved schema with {Result}",
                new { listId, columnCount = schema.Columns.Length, statusCount = schema.Statuses.Length });

            var result = new
            {
                success = true,
                listId,
                columns = schema.Columns.Select(col => new
                    {
                        name = col.Name,
                        property = col.Property,
                        type = col.Type.ToString()
                    })
                    .ToArray(),
                statuses = schema.Statuses.Select(status => new
                    {
                        name = status.Name,
                        color = status.Color
                    })
                    .ToArray()
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get schema with {Request}", new { listId });
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Read the change feed SSE stream and return recent events")]
    public static async Task<string> ReadChangeFeed(
        ListerApiClient apiClient,
        [Description("Maximum events to read (default 10)")] int maxEvents = 10,
        [Description("Timeout in seconds (default 5)")] int timeoutSeconds = 5,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("ReadChangeFeed called with {Request}", new { maxEvents, timeoutSeconds });
        try
        {
            var evts = await apiClient.ReadChangeFeedAsync(maxEvents, TimeSpan.FromSeconds(timeoutSeconds), cancellationToken);
            return JsonSerializer.Serialize(new { success = true, events = evts }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to read change feed");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Update a notification rule")]
    public static async Task<string> UpdateNotificationRule(
        ListerApiClient apiClient,
        [Description("Rule ID")] string ruleId,
        [Description("Trigger JSON")] string triggerJson,
        [Description("Channels JSON array")] string channelsJson,
        [Description("Schedule JSON")] string scheduleJson,
        [Description("Optional template ID")] string? templateId = null,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("UpdateNotificationRule called with {Request}", new { ruleId });
        try
        {
            if (!Guid.TryParse(ruleId, out var rid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid rule ID" }, JsonOptions);
            }

            var trigger = JsonSerializer.Deserialize<NotificationTrigger>(triggerJson, JsonOptions)
                          ?? NotificationTrigger.ItemCreated();
            var channels = JsonSerializer.Deserialize<NotificationChannel[]>(channelsJson, JsonOptions)
                           ?? new[] { NotificationChannel.InApp() };
            var schedule = JsonSerializer.Deserialize<NotificationSchedule>(scheduleJson, JsonOptions)
                           ?? NotificationSchedule.Immediate();

            await apiClient.UpdateNotificationRuleAsync(rid, trigger, channels, schedule, templateId,
                cancellationToken);
            return JsonSerializer.Serialize(new { success = true }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update notification rule");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Delete a notification rule")]
    public static async Task<string> DeleteNotificationRule(
        ListerApiClient apiClient,
        [Description("Rule ID")] string ruleId,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("DeleteNotificationRule called with {Request}", new { ruleId });
        try
        {
            if (!Guid.TryParse(ruleId, out var rid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid rule ID" }, JsonOptions);
            }

            await apiClient.DeleteNotificationRuleAsync(rid, cancellationToken);
            return JsonSerializer.Serialize(new { success = true }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete notification rule");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Create a notification rule (trigger, channels, schedule)")]
    public static async Task<string> CreateNotificationRule(
        ListerApiClient apiClient,
        [Description("The list ID the rule applies to")]
        string listId,
        [Description("Trigger JSON (e.g., {\"type\":\"ItemCreated\"})")]
        string triggerJson,
        [Description("Channels JSON array (e.g., [{\"type\":\"InApp\"},{\"type\":\"Email\",\"address\":\"a@b.com\"}])")]
        string channelsJson,
        [Description("Schedule JSON (e.g., {\"type\":\"Immediate\"})")]
        string scheduleJson,
        [Description("Optional template ID")] string? templateId = null,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("CreateNotificationRule called with {Request}", new { listId });
        try
        {
            if (!Guid.TryParse(listId, out var lid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID" }, JsonOptions);
            }

            var trigger = JsonSerializer.Deserialize<NotificationTrigger>(triggerJson, JsonOptions)
                          ?? NotificationTrigger.ItemCreated();
            var channels = JsonSerializer.Deserialize<NotificationChannel[]>(channelsJson, JsonOptions)
                           ?? new[] { NotificationChannel.InApp() };
            var schedule = JsonSerializer.Deserialize<NotificationSchedule>(scheduleJson, JsonOptions)
                           ?? NotificationSchedule.Immediate();

            var rule = await apiClient.CreateNotificationRuleAsync(lid, trigger, channels, schedule, templateId,
                cancellationToken);
            return JsonSerializer.Serialize(new { success = true, rule }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create notification rule");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Get configured status transitions for a list")]
    public static async Task<string> GetStatusTransitions(
        ListerApiClient apiClient,
        [Description("The ID of the list")] string listId,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("GetStatusTransitions called with {Request}", new { listId });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            var transitions = await apiClient.GetStatusTransitionsAsync(guid, cancellationToken);
            return JsonSerializer.Serialize(new { success = true, transitions }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get status transitions");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Set status transitions for a list")]
    public static async Task<string> SetStatusTransitions(
        ListerApiClient apiClient,
        [Description("The ID of the list")] string listId,
        [Description("JSON array of transitions: [{from, allowedNext: [..]}]")]
        string transitionsJson,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("SetStatusTransitions called with {Request}", new { listId });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            var transitions = JsonSerializer.Deserialize<StatusTransition[]>(transitionsJson, JsonOptions) ?? [];
            await apiClient.SetStatusTransitionsAsync(guid, transitions, cancellationToken);
            return JsonSerializer.Serialize(new { success = true }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to set status transitions");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Update a list item's bag (validates schema + transitions)")]
    public static async Task<string> UpdateListItem(
        ListerApiClient apiClient,
        [Description("The ID of the list")] string listId,
        [Description("The ID of the item")] int itemId,
        [Description("JSON object with updated bag")]
        string bagJson,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("UpdateListItem called with {Request}", new { listId, itemId });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            var bag = JsonSerializer.Deserialize<Dictionary<string, object?>>(bagJson, JsonOptions) ??
                      new Dictionary<string, object?>();
            await apiClient.UpdateListItemAsync(guid, itemId, bag, cancellationToken);
            return JsonSerializer.Serialize(new { success = true }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update list item");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    // Notifications
    [McpServerTool]
    [Description("Get notifications for current user (filterable)")]
    public static async Task<string> GetNotifications(
        ListerApiClient apiClient,
        [Description("ISO-8601 date to filter since")]
        string? since = null,
        [Description("Only unread if true")] bool? unread = null,
        [Description("Filter by list ID")] string? listId = null,
        [Description("Page size")] int pageSize = 20,
        [Description("Page index (0-based)")] int page = 0,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("GetNotifications called");
        try
        {
            DateTime? sinceDt = null;
            if (!string.IsNullOrEmpty(since) && DateTime.TryParse(since, out var d))
            {
                sinceDt = d;
            }

            Guid? listGuid = null;
            if (!string.IsNullOrEmpty(listId) && Guid.TryParse(listId, out var lg))
            {
                listGuid = lg;
            }

            var pageData = await apiClient.GetUserNotificationsAsync(sinceDt, unread, listGuid, pageSize, page,
                cancellationToken);
            return JsonSerializer.Serialize(new { success = true, page = pageData }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get notifications");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Get notification details by ID")]
    public static async Task<string> GetNotificationDetails(
        ListerApiClient apiClient,
        [Description("Notification ID")] string notificationId,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("GetNotificationDetails called with {Request}", new { notificationId });
        try
        {
            if (!Guid.TryParse(notificationId, out var id))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid ID" }, JsonOptions);
            }

            var details = await apiClient.GetNotificationDetailsAsync(id, cancellationToken);
            if (details is null)
            {
                return JsonSerializer.Serialize(new { success = false, error = "Not found" }, JsonOptions);
            }

            return JsonSerializer.Serialize(new { success = true, details }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get notification details");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Get unread notification count (optionally by list)")]
    public static async Task<string> GetUnreadCount(
        ListerApiClient apiClient,
        [Description("Optional list ID")] string? listId = null,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("GetUnreadCount called");
        try
        {
            Guid? lg = null;
            if (!string.IsNullOrEmpty(listId) && Guid.TryParse(listId, out var g))
            {
                lg = g;
            }

            var count = await apiClient.GetUnreadNotificationCountAsync(lg, cancellationToken);
            return JsonSerializer.Serialize(new { success = true, count }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get unread count");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Mark a notification as read")]
    public static async Task<string> MarkNotificationAsRead(
        ListerApiClient apiClient,
        [Description("Notification ID")] string notificationId,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("MarkNotificationAsRead called with {Request}", new { notificationId });
        try
        {
            if (!Guid.TryParse(notificationId, out var id))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid ID" }, JsonOptions);
            }

            await apiClient.MarkNotificationAsReadAsync(id, cancellationToken);
            return JsonSerializer.Serialize(new { success = true }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to mark notification as read");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Mark all notifications as read (optional before date)")]
    public static async Task<string> MarkAllNotificationsAsRead(
        ListerApiClient apiClient,
        [Description("Optional ISO-8601 cutoff")]
        string? before = null,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("MarkAllNotificationsAsRead called");
        try
        {
            DateTime? cutoff = null;
            if (!string.IsNullOrEmpty(before) && DateTime.TryParse(before, out var d))
            {
                cutoff = d;
            }

            await apiClient.MarkAllNotificationsAsReadAsync(cutoff, cancellationToken);
            return JsonSerializer.Serialize(new { success = true }, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to mark all notifications as read");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Get items from a list with optional filtering and pagination")]
    public static async Task<string> GetListItems(
        ListerApiClient apiClient,
        [Description("The ID of the list to get items from")]
        string listId,
        [Description("Page number (default: 1)")]
        int page = 1,
        [Description("Items per page (default: 20, max: 100)")]
        int pageSize = 20,
        [Description("Search term to filter items")]
        string? searchTerm = null,
        [Description("Column to sort by")] string? sortBy = null,
        [Description("Sort ascending (true) or descending (false)")]
        bool ascending = true,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("GetListItems called with {Request}",
            new { listId, page, pageSize, searchTerm });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            // Limit page size to prevent abuse
            pageSize = Math.Min(pageSize, 100);

            var pagedItems = await apiClient.GetListItemsAsync(guid, page, pageSize, searchTerm, sortBy, ascending,
                cancellationToken);
            Log.Information("Successfully retrieved items with {Result}",
                new { listId, page, itemCount = pagedItems.Items.Length, totalCount = pagedItems.Count });

            var result = new
            {
                success = true,
                listId,
                page,
                pageSize,
                totalCount = pagedItems.Count,
                items = pagedItems.Items.Select(item => new
                    {
                        id = item.Id,
                        listId = item.ListId,
                        data = item.Bag
                    })
                    .ToArray()
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get items with {Request}", new { listId });
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Get detailed information about a specific item including its history")]
    public static async Task<string> GetItemDetails(
        ListerApiClient apiClient,
        [Description("The ID of the list containing the item")]
        string listId,
        [Description("The ID of the item to get details for")]
        int itemId,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("GetItemDetails called with {Request}", new { listId, itemId });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            var itemDetails = await apiClient.GetItemDetailsAsync(guid, itemId, cancellationToken);
            Log.Information("Successfully retrieved item details with {Result}", new { listId, itemId });

            var result = new
            {
                success = true,
                item = new
                {
                    id = itemDetails.Id,
                    listId = itemDetails.ListId,
                    bag = itemDetails.Bag
                    // history = itemDetails.History.Select(h => new
                    //     {
                    //         timestamp = h.On,
                    //         action = h.Type.ToString(),
                    //         user = h.By
                    //     })
                    //     .ToArray()
                }
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get item details with {Request}", new { listId, itemId });
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Create a new item in a list with structured data")]
    public static async Task<string> CreateListItem(
        ListerApiClient apiClient,
        [Description("The ID of the list to add the item to")]
        string listId,
        [Description("JSON object containing the item data matching the list schema")]
        string itemData,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("CreateListItem called with {Request}", new { listId, itemData });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            var data = JsonSerializer.Deserialize<Dictionary<string, object?>>(itemData, JsonOptions);
            if (data == null)
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid item data JSON" }, JsonOptions);
            }

            var createdItem = await apiClient.CreateListItemAsync(guid, data, cancellationToken);
            Log.Information("Successfully created item with {Result}", new { listId, itemId = createdItem.Id });

            var result = new
            {
                success = true,
                item = new
                {
                    id = createdItem.Id,
                    listId = createdItem.ListId,
                    data = createdItem.Bag
                }
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create item with {Request}", new { listId, itemData });
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Create a new list with specified columns and statuses")]
    public static async Task<string> CreateList(
        ListerApiClient apiClient,
        [Description("Name of the new list")] string name,
        [Description("JSON array of column definitions with name, property, and type")]
        string columnsJson,
        [Description("JSON array of status definitions with name and color")]
        string statusesJson,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("CreateList called with {Request}", new { name });
        try
        {
            var columns = JsonSerializer.Deserialize<Column[]>(columnsJson, JsonOptions);
            var statuses = JsonSerializer.Deserialize<Status[]>(statusesJson, JsonOptions);

            if (columns == null || statuses == null)
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid columns or statuses JSON" },
                    JsonOptions);
            }

            var createdList = await apiClient.CreateListAsync(name, columns, statuses, cancellationToken);
            Log.Information("Successfully created list with {Result}",
                new { listId = createdList.Id, name = createdList.Name });

            var result = new
            {
                success = true,
                list = new
                {
                    id = createdList.Id,
                    name = createdList.Name,
                    itemCount = createdList.Count
                }
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create list with {Request}", new { name });
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Delete an item from a list")]
    public static async Task<string> DeleteListItem(
        ListerApiClient apiClient,
        [Description("The ID of the list containing the item")]
        string listId,
        [Description("The ID of the item to delete")]
        int itemId,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("DeleteListItem called with {Request}", new { listId, itemId });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            await apiClient.DeleteListItemAsync(guid, itemId, cancellationToken);
            Log.Information("Successfully deleted item with {Result}", new { listId, itemId });

            var result = new
            {
                success = true,
                message = $"Item {itemId} deleted successfully"
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete item with {Request}", new { listId, itemId });
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Delete a list")]
    public static async Task<string> DeleteList(
        ListerApiClient apiClient,
        [Description("The ID of the list to delete")]
        string listId,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("DeleteList called with {Request}", new { listId });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            await apiClient.DeleteListAsync(guid, cancellationToken);
            Log.Information("Successfully deleted list with {Result}", new { listId });

            var result = new
            {
                success = true,
                message = $"List {listId} deleted successfully"
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete list with {Request}", new { listId });
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }

    [McpServerTool]
    [Description("Get the Swagger/OpenAPI specification for the Lister API")]
    public static async Task<string> GetSwaggerJson(
        ListerApiClient apiClient,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("GetSwaggerJson called");
        try
        {
            var response = await apiClient.GetSwaggerJsonAsync(cancellationToken);
            Log.Information("Successfully retrieved swagger.json with {Result}", new { length = response.Length });

            var result = new
            {
                success = true,
                swaggerJson = JsonSerializer.Deserialize<JsonElement>(response)
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to retrieve swagger.json");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message }, JsonOptions);
        }
    }
}
