using Lister.Core.Application;

namespace Lister.Lists.Application.Commands.DeleteList;

public record DeleteListCommand(string ListId) : RequestBase;