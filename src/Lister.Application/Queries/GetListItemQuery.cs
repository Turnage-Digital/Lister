using Lister.Core.Entities;

namespace Lister.Application.Queries;

public record GetListItemQuery(string ListId, int ItemId) : RequestBase<Item?>;