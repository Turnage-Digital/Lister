using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListByIdQuery<TList> : IRequest<TList>
    where TList : IReadOnlyList
{
    public GetListByIdQuery(string userId, Guid id)
    {
        Id = id;
        UserId = userId;
    }

    public string UserId { get; }

    public Guid Id { get; }
}