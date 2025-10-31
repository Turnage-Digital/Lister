using Lister.Core.Application;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Commands.ConvertTextToListItem;

public record ConvertTextToListItemCommand(Guid ListId, string Text)
    : RequestBase<ListItemDto>;