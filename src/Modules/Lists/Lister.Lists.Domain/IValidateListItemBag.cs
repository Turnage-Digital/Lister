using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Domain;

public interface IValidateListItemBag<in TList>
    where TList : IWritableList
{
    Task ValidateAsync(TList list, object bag, CancellationToken cancellationToken);
}
