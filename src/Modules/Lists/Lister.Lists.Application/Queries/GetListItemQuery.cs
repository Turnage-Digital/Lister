using Lister.Core.Application;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Application.Queries;

public record GetListItemQuery(string ListId, int ItemId) : RequestBase<Item?>;