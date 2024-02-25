using Lister.Core;
using MediatR;

namespace Lister.Endpoints.Lists.GetListNames;

public class GetListNamesQuery<TList> : RequestBase<TList[]>
    where TList : IReadOnlyList
{
    public GetListNamesQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }
}