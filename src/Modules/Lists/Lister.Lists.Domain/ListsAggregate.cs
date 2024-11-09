using System.Dynamic;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Events;
using Lister.Lists.Domain.ValueObjects;
using MediatR;

namespace Lister.Lists.Domain;

public class ListsAggregate<TList>(IListsUnitOfWork<TList> unitOfWork, IMediator mediator)
    where TList : IWritableList
{
    public async Task<TList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.GetByIdAsync(id, cancellationToken);
        return retval;
    }

    public async Task<TList?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.GetByNameAsync(name, cancellationToken);
        return retval;
    }

    public async Task<TList> CreateAsync(
        string createdBy,
        string name,
        IEnumerable<Status> statuses,
        IEnumerable<Column> columns,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await unitOfWork.ListsStore.InitAsync(name, createdBy, cancellationToken);
        await unitOfWork.ListsStore.SetColumnsAsync(retval, columns, cancellationToken);
        await unitOfWork.ListsStore.SetStatusesAsync(retval, statuses, cancellationToken);
        await unitOfWork.ListsStore.CreateAsync(retval, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ListCreatedEvent(retval, createdBy), cancellationToken);
        return retval;
    }

    public async Task DeleteAsync(TList list, string deletedBy, CancellationToken cancellationToken = default)
    {
        await unitOfWork.ListsStore.DeleteAsync(list, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ListDeletedEvent(list, deletedBy), cancellationToken);
    }

    public async Task<Item?> GetListItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.GetItemByIdAsync(id, cancellationToken);
        return retval;
    }
    
    public async Task<Item> CreateListItemAsync(
        TList list,
        object bag,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.CreateItemAsync(list, bag, createdBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ListItemCreatedEvent(retval, createdBy), cancellationToken);
        return retval;
    }

    public async Task<IEnumerable<Item>> CreateListItemsAsync(
        TList list,
        IEnumerable<object> bags,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        var retval = new List<Item>();
        foreach (var bag in bags)
        {
            var item = await unitOfWork.ListsStore.CreateItemAsync(list, bag, createdBy, cancellationToken);
            retval.Add(item);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (var item in retval)
        {
            await mediator.Publish(new ListItemCreatedEvent(item, createdBy), cancellationToken);
        }
        return retval;
    }

    public async Task DeleteListItemAsync(
        TList list,
        Item item,
        string deletedBy,
        CancellationToken cancellationToken = default)
    {
        await unitOfWork.ListsStore.DeleteItemAsync(list, item, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ListItemDeletedEvent(item, deletedBy), cancellationToken);
    }

    public async Task<object> CreateExampleBagAsync(TList list, CancellationToken cancellationToken = default)
    {
        dynamic retval = new ExpandoObject();
        var typeMap = new Dictionary<ColumnType, object>
        {
            { ColumnType.Text, "text" },
            { ColumnType.Number, 77 },
            { ColumnType.Date, DateTime.Today.ToString("O") },
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