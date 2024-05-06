using Lister.Application;
using Lister.Core.ValueObjects;

namespace Lister.Endpoints.Lists.GetListItems;

public class GetListItemsResponse(IEnumerable<Item> items, long count)
    : PagedResponseBase<Item>(items, count);