using Lister.Core;

namespace Lister.Application.Queries;

public class GetListItemDefinitionQuery<TList>(string listId) : RequestBase<TList>
    where TList : IReadOnlyList?
{
    public string ListId { get; } = listId;
}