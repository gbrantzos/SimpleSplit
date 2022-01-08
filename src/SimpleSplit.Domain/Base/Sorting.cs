using System.Linq.Expressions;

namespace SimpleSplit.Domain.Base
{
    public abstract class Sorting
    {
        public enum SortDirection
        {
            Ascending,
            Descending
        }
    }

    public class Sorting<T> : Sorting
    {
        public SortDirection Direction { get; init; }
        public Expression<Func<T, object>> Expression { get; init; }

        public static readonly IEnumerable<Sorting<T>> EmptySorting = new Sorting<T>[] { };
    }
}