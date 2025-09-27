using Lister.Core.Application;

namespace Lister.Lists.Application.Endpoints.UpdateListItem;

public record UpdateListItemCommand(Guid ListId, int ItemId, object Bag) : RequestBase;