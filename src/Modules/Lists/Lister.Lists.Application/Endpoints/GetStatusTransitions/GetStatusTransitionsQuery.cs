using Lister.Core.Application;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Application.Endpoints.GetStatusTransitions;

public record GetStatusTransitionsQuery(Guid ListId) : RequestBase<StatusTransition[]>;