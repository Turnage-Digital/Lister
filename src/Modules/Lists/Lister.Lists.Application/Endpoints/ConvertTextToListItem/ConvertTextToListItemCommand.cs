using Lister.Core.Application;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Application.Endpoints.ConvertTextToListItem;

public record ConvertTextToListItemCommand(string ListId, string Text) : RequestBase<Item>;