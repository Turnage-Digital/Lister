using Lister.Domain.Views;

namespace Lister.Application.Queries;

public record GetListNamesQuery : RequestBase<ListName[]>;