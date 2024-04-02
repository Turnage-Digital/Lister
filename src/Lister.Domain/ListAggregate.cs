using System.Dynamic;
using Lister.Core;
using Lister.Core.Enums;
using Lister.Core.ValueObjects;
using Lister.Domain.Events;
using MediatR;
using Serilog;

namespace Lister.Domain;

public class ListAggregate<TList>
    where TList : IWritableList
{
    private readonly IMediator _mediator;
    private readonly IListerUnitOfWork<TList> _unitOfWork;

    public ListAggregate(IListerUnitOfWork<TList> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<TList> CreateAsync(
        string createdBy,
        string name,
        IEnumerable<Status> statuses,
        IEnumerable<Column> columns,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _unitOfWork.ListsStore.InitAsync(createdBy, name, cancellationToken);
        await _unitOfWork.ListsStore.SetColumnsAsync(retval, columns, cancellationToken);
        await _unitOfWork.ListsStore.SetStatusesAsync(retval, statuses, cancellationToken);
        await _unitOfWork.ListsStore.CreateAsync(retval, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Log.Information("Created list: {list}", retval);
        await _mediator.Publish(new ListCreatedEvent(retval.Id!.Value), cancellationToken);
        return retval;
    }

    public async Task<TList?> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        var retval = await _unitOfWork.ListsStore.ReadAsync(id, cancellationToken);
        return retval;
    }

    public async Task<TList?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var retval = await _unitOfWork.ListsStore.FindByNameAsync(name, cancellationToken);
        return retval;
    }

    public async Task<Item> CreateItemAsync(TList list, object bag, CancellationToken cancellationToken = default)
    {
        var retval = await _unitOfWork.ListsStore.InitItemAsync(list, bag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Log.Information("Created list item: {item}", retval);
        await _mediator.Publish(new ListItemCreatedEvent(retval.Id!.Value), cancellationToken);
        return retval;
    }
    
    // {
    //     "listId": "08dc44fa-810f-4740-8e5a-ad8e1dace60d",
    //     "text": "This new Trailer Park Boy we have planned is Greasy Lee. He's located in Spring Hill, Florida.
    //          The street address for this guy is 101 Maple Lane, he's bunking with Ricky. DOB is 2/2/1978."
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

        var columns = await _unitOfWork.ListsStore.GetColumnsAsync(list, cancellationToken);
        foreach (var column in columns)
        {
            ((IDictionary<string, object?>)retval)[column.Property] = typeMap[column.Type];
        }

        var statuses = await _unitOfWork.ListsStore.GetStatusesAsync(list, cancellationToken);
        retval.status = statuses.First().Name;

        return retval;
    }
}