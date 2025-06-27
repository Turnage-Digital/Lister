using Lister.Core.Application;

namespace Lister.Lists.Application.Endpoints.DeleteListItem;

public record DeleteListItemCommand(string ListId, int ItemId) 
    : RequestBase;