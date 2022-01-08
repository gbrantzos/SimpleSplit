using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Services
{
    /// <summary>
    /// <para>
    /// Sorting information parser.
    /// </para>
    /// Creates <see cref="Sorting{T}"/> instances parsing string representation of sorting details.
    /// </summary>
    public interface ISortingParser
    {
        /// <summary>
        /// <para>
        /// Parse sorting information.
        /// </para>
        /// <example>
        /// Sorting information <paramref name="sortingDetails"/> is an enumerable of string, for example:
        /// <code>
        ///     +enteredAt,id
        ///     -description
        /// </code>
        /// Multiple fields can be separated by commas. Character '+' or '-' in the beginning of a field
        /// define sort ordering (ascending or descending).
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortingDetails"></param>
        /// <returns></returns>
        IEnumerable<Sorting<T>> BuildSorting<T>(IEnumerable<string> sortingDetails);
    }
}
