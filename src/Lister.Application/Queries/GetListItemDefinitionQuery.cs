using Lister.Domain.Views;

namespace Lister.Application.Queries.List;

public record GetListItemDefinitionQuery(string ListId) : RequestBase<ListItemDefinition?>;