using Lister.Core.Application;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Endpoints.SetStatusTransitions;

public record SetStatusTransitionsCommand(Guid ListId, StatusTransition[] Transitions) : RequestBase;

