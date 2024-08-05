using Lister.Core;

namespace Lister.Application.Queries;

public record GetListItemDefinitionQuery<TList>(string ListId) : RequestBase<TList>
    where TList : IReadOnlyList?;