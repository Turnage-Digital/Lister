namespace Lister.Core.Entities;

public interface IList
{
    Guid? Id { get; set; }
}

public interface IReadOnlyList : IList;

public interface IWritableList : IList;