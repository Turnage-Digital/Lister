using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Endpoints.GetListNames;

public record GetListNamesQuery : RequestBase<ListName[]>;