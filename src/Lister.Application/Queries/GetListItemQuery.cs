using Lister.Core.ValueObjects;

namespace Lister.Application.Queries;

public record GetListItemQuery(string ListId, int ItemId) : RequestBase<Item?>;