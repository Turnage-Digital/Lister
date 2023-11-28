using MediatR;

namespace Lister.Application.Queries;

public class GetListNamesQuery<T> : IRequest<T[]>
{
    public GetListNamesQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }
}