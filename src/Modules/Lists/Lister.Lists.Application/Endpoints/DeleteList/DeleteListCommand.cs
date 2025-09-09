using Lister.Core.Application;

namespace Lister.Lists.Application.Endpoints.DeleteList;

public record DeleteListCommand(string ListId)
    : RequestBase;