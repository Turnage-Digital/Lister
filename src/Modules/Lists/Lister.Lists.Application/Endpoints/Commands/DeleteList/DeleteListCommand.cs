using Lister.Core.Application;

namespace Lister.Lists.Application.Endpoints.Commands.DeleteList;

public record DeleteListCommand(Guid ListId)
    : RequestBase;