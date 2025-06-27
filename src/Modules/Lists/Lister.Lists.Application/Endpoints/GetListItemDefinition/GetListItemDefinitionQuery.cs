using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Endpoints.GetListItemDefinition;

public record GetListItemDefinitionQuery(string ListId) : RequestBase<ListItemDefinition?>;