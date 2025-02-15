using Lister.Core.Application;

namespace Lister.Lists.Application.Commands.DeleteListItem;

public record DeleteListItemCommand(string ListId, int ItemId) : RequestBase;