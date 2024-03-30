using Lister.Core;

namespace Lister.Endpoints.Lists.GetListItemDefinition;

public class GetListItemDefinitionQuery<TList> : RequestBase<TList>
    where TList : IReadOnlyList
{
    public GetListItemDefinitionQuery(string listId)
    {
        ListId = listId;
    }

    public string ListId { get; }
}