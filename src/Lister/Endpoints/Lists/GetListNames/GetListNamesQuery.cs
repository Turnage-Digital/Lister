using Lister.Core;

namespace Lister.Endpoints.Lists.GetListNames;

public class GetListNamesQuery<TList> : RequestBase<TList[]>
    where TList : IReadOnlyList;