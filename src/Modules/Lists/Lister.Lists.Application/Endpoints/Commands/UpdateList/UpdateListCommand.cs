using Lister.Core.Application;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Endpoints.Commands.UpdateList;

public record UpdateListCommand(
    Guid ListId,
    Column[]? Columns,
    Status[]? Statuses,
    StatusTransition[]? Transitions
) : RequestBase;