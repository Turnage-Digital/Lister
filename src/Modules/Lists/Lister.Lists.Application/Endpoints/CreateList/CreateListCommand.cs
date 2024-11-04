using Lister.Core.Application;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Domain.Views;

namespace Lister.Lists.Application.Endpoints.CreateList;

public record CreateListCommand(string Name, Status[] Statuses, Column[] Columns) : RequestBase<ListItemDefinition>;