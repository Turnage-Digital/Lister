using Lister.Core;
using Lister.Core.Entities;

namespace Lister.Application.Queries;

public record GetListItemDefinitionQuery<TList>(string ListId) : RequestBase<TList>
    where TList : IReadOnlyList?;