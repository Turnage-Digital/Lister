using Lister.Lists.Domain.Entities;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Mappings;

internal static class ListItemWriteContextMap
{
    public static ListItemDto ToDto(IWritableItem item, object bag) =>
        new()
        {
            Id = item.Id,
            ListId = item.ListId,
            Bag = bag
        };
}
