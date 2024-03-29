using Lister.Core;

namespace Lister.Endpoints.Lists.GetListItemDefinitionById;

public class GetListItemDefinitionByIdQuery<TList> : RequestBase<TList>
    where TList : IReadOnlyList
{
    public GetListItemDefinitionByIdQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }
}