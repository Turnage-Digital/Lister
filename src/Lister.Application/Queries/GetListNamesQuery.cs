using Lister.Core;
using Lister.Core.Entities;

namespace Lister.Application.Queries;

public record GetListNamesQuery<TList> : RequestBase<TList[]>
    where TList : IReadOnlyList;