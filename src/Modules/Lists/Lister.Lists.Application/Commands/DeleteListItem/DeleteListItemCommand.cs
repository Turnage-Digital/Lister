using Lister.Core.Application;

namespace Lister.Lists.Application.Commands.DeleteListItem;

public record DeleteListItemCommand(Guid ListId, int ItemId)
    : RequestBase;