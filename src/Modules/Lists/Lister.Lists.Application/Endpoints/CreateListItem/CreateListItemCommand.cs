using Lister.Core.Application;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Application.Endpoints.CreateListItem;

public record CreateListItemCommand(string ListId, object Bag) : RequestBase<Item>;