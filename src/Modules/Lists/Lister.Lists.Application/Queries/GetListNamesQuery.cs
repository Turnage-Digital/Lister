using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Queries;

public record GetListNamesQuery : RequestBase<ListName[]>;