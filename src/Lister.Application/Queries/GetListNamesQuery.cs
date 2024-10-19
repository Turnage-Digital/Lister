using Lister.Domain.Views;

namespace Lister.Application.Queries.List;

public record GetListNamesQuery : RequestBase<ListName[]>;