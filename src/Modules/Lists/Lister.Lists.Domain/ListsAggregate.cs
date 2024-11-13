using System.Dynamic;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Events;
using Lister.Lists.Domain.ValueObjects;
using MediatR;

namespace Lister.Lists.Domain;

public class ListsAggregate<TList, TItem>(IListsUnitOfWork<TList, TItem> unitOfWork, IMediator mediator)
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<TList?> GetListByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.GetByIdAsync(id, cancellationToken);
        return retval;
    }

    public async Task<TList?> GetListByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var retval = await unitOfWork.ListsStore.GetByNameAsync(name, cancellationToken);
        return retval;
    }

    public async Task<TList> CreateListAsync(
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

    public async Task DeleteListAsync(TList list, string deletedBy, CancellationToken cancellationToken = default)
    {
        await unitOfWork.ListsStore.DeleteAsync(list, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ListDeletedEvent(list, deletedBy), cancellationToken);
    }

    public async Task<TItem?> GetItemByIdAsync(
        TList list,
        int itemId,
        CancellationToken cancellationToken = default)
    {
        if (list.Id is null)
        {
            throw new InvalidOperationException("List must have an Id");
        }

        var retval = await unitOfWork.ItemsStore.GetByIdAsync(itemId, list.Id.Value, cancellationToken);
        return retval;
    }

    public async Task<TItem> CreateItemAsync(
        TList list,
        object bag,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        if (list.Id is null)
        {
            throw new InvalidOperationException("List must have an Id");
        }

        var retval = await unitOfWork.ItemsStore.InitAsync(list.Id.Value, createdBy, cancellationToken);
        await unitOfWork.ItemsStore.SetBagAsync(retval, bag, cancellationToken);
        await unitOfWork.ItemsStore.CreateAsync(retval, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ListItemCreatedEvent(retval, createdBy), cancellationToken);
        return retval;
    }

    public async Task<IEnumerable<TItem>> CreateItemsAsync(
        TList list,
        IEnumerable<object> bags,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        if (list.Id is null)
        {
            throw new InvalidOperationException("List must have an Id");
        }

        var retval = new List<TItem>();
        foreach (var bag in bags)
        {
            var item = await unitOfWork.ItemsStore.InitAsync(list.Id.Value, createdBy, cancellationToken);
            await unitOfWork.ItemsStore.SetBagAsync(item, bag, cancellationToken);
            await unitOfWork.ItemsStore.CreateAsync(item, cancellationToken);
            retval.Add(item);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (var item in retval)
        {
            await mediator.Publish(new ListItemCreatedEvent(item, createdBy), cancellationToken);
        }

        return retval;
    }

    public async Task DeleteItemAsync(
        TItem item,
        string deletedBy,
        CancellationToken cancellationToken = default)
    {
        await unitOfWork.ItemsStore.DeleteAsync(item, deletedBy, cancellationToken);
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