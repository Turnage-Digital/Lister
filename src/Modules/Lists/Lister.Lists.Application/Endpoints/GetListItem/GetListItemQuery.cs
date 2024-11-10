using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Endpoints.GetListItem;

public record GetListItemQuery(string ListId, int ItemId) : RequestBase<ItemDetails?>;