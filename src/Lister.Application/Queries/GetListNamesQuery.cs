using Lister.Core;

namespace Lister.Application.Queries;

public record GetListNamesQuery<TList> : RequestBase<TList[]>
    where TList : IReadOnlyList;