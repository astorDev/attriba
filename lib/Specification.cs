using System.Linq.Expressions;
using LinqKit;
namespace Attriba;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
}

public class AndSpecification<T>(IEnumerable<ISpecification<T>> Children) : ISpecification<T>
{
    public Expression<Func<T, bool>> ToExpression()
    {
        var predicate = PredicateBuilder.New<T>(true);
        foreach (var child in Children)
        {
            var expression = child.ToExpression();
            predicate = predicate.And(expression);
        }

        return predicate;
    }
}

public class OrSpecification<T>(IEnumerable<ISpecification<T>> Children) : ISpecification<T>
{
    public Expression<Func<T, bool>> ToExpression()
    {
        var predicate = PredicateBuilder.New<T>(false);
        foreach (var child in Children)
        {
            var expression = child.ToExpression();
            predicate = predicate.Or(expression);
        }

        return predicate;
    }
}

public static class QueryableExtensions
{
    public static IQueryable<T> Where<T>(this IQueryable<T> source, ISpecification<T> specification) => 
        source.Where(specification.ToExpression());
}