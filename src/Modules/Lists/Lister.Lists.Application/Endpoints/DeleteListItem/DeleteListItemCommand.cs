using Lister.Core.Application;

namespace Lister.Lists.Application.Endpoints.DeleteListItem;

public record DeleteListItemCommand(Guid ListId, int ItemId)
    : RequestBase;