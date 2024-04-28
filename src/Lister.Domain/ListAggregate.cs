using System.Dynamic;
using Lister.Core;
using Lister.Core.Enums;
using Lister.Core.ValueObjects;
using Lister.Domain.Events;
using MediatR;
using Serilog;

namespace Lister.Domain;

public class ListAggregate<TList>(IListerUnitOfWork<TList> unitOfWork, IMediator mediator)
    where TList : IWritableList
{
    public async Task<TList> CreateAsync(
        string createdBy,
        string name,
        IEnumerable<Status> statuses,
        IEnumerable<Column> columns,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await unitOfWork.ListsStore.InitAsync(createdBy, name, cancellationToken);
        await unitOfWork.ListsStore.SetColumnsAsync(retval, columns, cancellationToken);
        await unitOfWork.ListsStore.SetStatusesAsync(retval, statuses, cancellationToken);
        await unitOfWork.ListsStore.CreateAsync(retval, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        Log.Information("Created list: {list}", retval);
        await mediator.Publish(new ListCreatedEvent(retval.Id!.Value), cancellationToken);
        return retval;
    }

    public async Task<TList?> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.ReadAsync(id, cancellationToken);
        return retval;
    }

    public async Task<TList?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.FindByNameAsync(name, cancellationToken);
        return retval;
    }

    public async Task<Item> CreateItemAsync(
        TList list,
        string createdBy,
        object bag,
        CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.InitItemAsync(list, createdBy, bag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        Log.Information("Created list item: {item}", retval);
        await mediator.Publish(new ListItemCreatedEvent(retval.Id!.Value), cancellationToken);
        return retval;
    }

    // {
    //     "listId": "08dc44fa-810f-4740-8e5a-ad8e1dace60d",
    //     "text": "The new Trailer Park Boy is Jim Lahey. He's located in Spring Hill, Florida.
    //          The street address for this guy is 101 Maple Lane, he's bunking with Randy. DOB is 2/2/1928."
    // }
    public async Task<object> CreateExampleBagAsync(TList list, CancellationToken cancellationToken = default)
    {
        dynamic retval = new ExpandoObject();
        var typeMap = new Dictionary<ColumnType, object>
        {
            { ColumnType.Text, "text" },
            { ColumnType.Number, 42 },
            { ColumnType.Date, DateTime.Today.ToString("o") },
            { ColumnType.Boolean, false }
        };

        var columns = await unitOfWork.ListsStore.GetColumnsAsync(list, cancellationToken);
        foreach (var column in columns) ((IDictionary<string, object?>)retval)[column.Property] = typeMap[column.Type];

        var statuses = await unitOfWork.ListsStore.GetStatusesAsync(list, cancellationToken);
        retval.status = statuses.First().Name;

        return retval;
    }
}