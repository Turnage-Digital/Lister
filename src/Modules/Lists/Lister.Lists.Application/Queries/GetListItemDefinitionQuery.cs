using Lister.Core.Application;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Queries;

public record GetListItemDefinitionQuery(string ListId) : RequestBase<ListItemDefinition?>;