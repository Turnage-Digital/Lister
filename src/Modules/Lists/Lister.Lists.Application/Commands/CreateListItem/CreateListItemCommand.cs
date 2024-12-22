using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Commands.CreateListItem;

public record CreateListItemCommand(string ListId, object Bag) : RequestBase<ListItem
>;