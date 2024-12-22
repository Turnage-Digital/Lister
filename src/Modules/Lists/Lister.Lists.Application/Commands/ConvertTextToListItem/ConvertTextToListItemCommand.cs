using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Commands.ConvertTextToListItem;

public record ConvertTextToListItemCommand(string ListId, string Text) : RequestBase<ListItem>;