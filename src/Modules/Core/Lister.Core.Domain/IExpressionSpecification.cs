using System.Linq.Expressions;

namespace Lister.Core.Domain;

public interface IExpressionSpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}