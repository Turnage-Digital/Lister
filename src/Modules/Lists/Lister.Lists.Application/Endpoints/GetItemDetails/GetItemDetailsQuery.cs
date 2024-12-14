using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Endpoints.GetItemDetails;

public record GetItemDetailsQuery(string ListId, int ItemId) : RequestBase<ItemDetails?>;