using Lister.Domain.Entities;

namespace Lister.Application.Queries.List;

public record GetListItemQuery(string ListId, int ItemId) : RequestBase<Item?>;