using MediatR;

namespace Lister.Application.Commands;

public class DeleteListCommand : RequestBase<Unit>
{
    public Guid ListId { get; set; }
}