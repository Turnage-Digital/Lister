using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lister.Lists.Domain.ValueObjects;
using Lister.Mcp.Server.Services;
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
            Log.Information("Successfully retrieved {ListCount} lists", lists.Length);

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
                    data = itemDetails.Bag,
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

            // Parse the item data JSON
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
    [Description("Create a new item using AI to parse natural language text into structured data")]
    public static async Task<string> SmartCreateItem(
        ListerApiClient apiClient,
        [Description("The ID of the list to add the item to")]
        string listId,
        [Description("Natural language text describing the item to create")]
        string text,
        CancellationToken cancellationToken = default
    )
    {
        Log.Information("SmartCreateItem called with {Request}", new { listId, text });
        try
        {
            if (!Guid.TryParse(listId, out var guid))
            {
                return JsonSerializer.Serialize(new { success = false, error = "Invalid list ID format" }, JsonOptions);
            }

            var createdItem = await apiClient.SmartCreateItemAsync(guid, text, cancellationToken);
            Log.Information("Successfully smart-created item with {Result}", 
                new { listId, itemId = createdItem.Id });

            var result = new
            {
                success = true,
                sourceText = text,
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
            Log.Error(ex, "Failed to smart-create item with {Request}", new { listId, text });
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
}