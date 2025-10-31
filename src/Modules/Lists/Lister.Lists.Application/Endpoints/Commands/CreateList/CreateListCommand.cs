using Lister.Core.Application;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.Application.Endpoints.Commands.CreateList;

public record CreateListCommand(string Name, Status[] Statuses, Column[] Columns, StatusTransition[]? Transitions)
    : RequestBase<ListItemDefinitionDto>;