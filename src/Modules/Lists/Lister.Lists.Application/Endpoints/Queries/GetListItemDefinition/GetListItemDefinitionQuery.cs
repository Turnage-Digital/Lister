using Lister.Core.Application;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Endpoints.Queries.GetListItemDefinition;

public record GetListItemDefinitionQuery(Guid ListId) : RequestBase<ListItemDefinitionDto?>;