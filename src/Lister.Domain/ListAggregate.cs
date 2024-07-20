using System.Dynamic;
using Lister.Core;
using Lister.Core.Enums;
using Lister.Core.ValueObjects;
using Lister.Domain.Events;
using MediatR;

namespace Lister.Domain;

public class ListAggregate<TList, TItem>(IListerUnitOfWork<TList, TItem> unitOfWork, IMediator mediator)
    where TList : IWritableList
    where TItem : Item
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
        await mediator.Publish(new ListCreatedEvent(retval.Id!.Value, createdBy), cancellationToken);
        return retval;
    }

    public async Task<TList?> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.ReadAsync(id, cancellationToken);
        return retval;
    }

    public async Task DeleteAsync(TList list, string deletedBy, CancellationToken cancellationToken = default)
    {
        await unitOfWork.ListsStore.DeleteAsync(list, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ListDeletedEvent(list.Id!.Value, deletedBy), cancellationToken);
    }

    public async Task<TList?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.FindByNameAsync(name, cancellationToken);
        return retval;
    }

    public async Task<Item> AddListItemAsync(
        TList list,
        string createdBy,
        object bag,
        CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ItemsStore.InitAsync(createdBy, bag, cancellationToken);
        await unitOfWork.ListsStore.AddItemAsync(list, retval, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ListItemAddedEvent(retval.Id!.Value, createdBy), cancellationToken);
        return retval;
    }

    public async Task<IEnumerable<Item>> AddListItemsAsync(
        TList list,
        string createdBy,
        IEnumerable<object> bags,
        CancellationToken cancellationToken = default)
    {
        var retval = new List<Item>();
        foreach (var bag in bags)
        {
            var item = await unitOfWork.ItemsStore.InitAsync(createdBy, bag, cancellationToken);
            await unitOfWork.ListsStore.AddItemAsync(list, item, cancellationToken);
            retval.Add(item);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var ids = retval.Select(i => i.Id!.Value);
        await mediator.Publish(new ListItemsAddedEvent(ids, createdBy), cancellationToken);
        return retval;
    }

    public async Task<object> CreateExampleBagAsync(TList list, CancellationToken cancellationToken = default)
    {
        dynamic retval = new ExpandoObject();
        var typeMap = new Dictionary<ColumnType, object>
        {
            { ColumnType.Text, "text" },
            { ColumnType.Number, 77 },
            { ColumnType.Date, DateTime.Today.ToString("o") },
            { ColumnType.Boolean, true }
        };

        var columns = await unitOfWork.ListsStore.GetColumnsAsync(list, cancellationToken);
        foreach (var column in columns)
        {
            var cast = (IDictionary<string, object?>)retval;
            cast[column.Property] = typeMap[column.Type];
        }

        var statuses = await unitOfWork.ListsStore.GetStatusesAsync(list, cancellationToken);
        retval.status = statuses.First().Name;
        return retval;
    }
}