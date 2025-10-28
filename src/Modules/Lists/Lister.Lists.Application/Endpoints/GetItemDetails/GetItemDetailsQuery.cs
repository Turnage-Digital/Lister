using Lister.Core.Application;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Endpoints.GetItemDetails;

public record GetItemDetailsQuery(Guid ListId, int ItemId)
    : RequestBase<ItemDetailsDto?>;