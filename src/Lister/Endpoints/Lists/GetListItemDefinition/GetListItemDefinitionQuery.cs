using Lister.App;
using Lister.Core;

namespace Lister.Endpoints.Lists.GetListItemDefinition;

public class GetListItemDefinitionQuery<TList>(string listId) : RequestBase<TList>
    where TList : IReadOnlyList
{
    public string ListId { get; } = listId;
}