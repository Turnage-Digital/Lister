using Lister.Core.Application;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Endpoints.CreateListItem;

public record CreateListItemCommand(Guid ListId, object Bag)
    : RequestBase<ListItemDto>;