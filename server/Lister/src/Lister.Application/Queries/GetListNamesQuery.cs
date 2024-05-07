using Lister.Core;

namespace Lister.Application.Queries;

public class GetListNamesQuery<TList> : RequestBase<TList[]>
    where TList : IReadOnlyList;