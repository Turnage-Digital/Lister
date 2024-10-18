using Lister.Domain.Views;

namespace Lister.Application.Queries;

public record GetListItemDefinitionQuery(string ListId) : RequestBase<ListItemDefinition?>;