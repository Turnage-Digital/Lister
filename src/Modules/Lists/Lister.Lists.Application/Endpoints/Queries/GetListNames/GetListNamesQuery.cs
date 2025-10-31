using Lister.Core.Application;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Endpoints.Queries.GetListNames;

public record GetListNamesQuery : RequestBase<ListNameDto[]>;