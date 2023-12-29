using Lister.Core;
using MediatR;

namespace Lister.Application.Queries;

public class GetListItemDefinitionByIdQuery<TList> : IRequest<TList>
    where TList : IReadOnlyList
{
    public GetListItemDefinitionByIdQuery(string userId, Guid id)
    {
        Id = id;
        UserId = userId;
    }

    public string UserId { get; }

    public Guid Id { get; }
}