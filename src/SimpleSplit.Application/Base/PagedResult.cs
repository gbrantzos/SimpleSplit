using System.Text.Json.Serialization;

namespace SimpleSplit.Application.Base
{
    public abstract class PagedResult
    {
        /// <summary>
        /// Current page
        /// </summary>
        public int CurrentPage { get; init; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; init; }

        /// <summary>
        /// Total number of rows
        /// </summary>
        public int TotalRows { get; init; }
    }

    public class PagedResult<T> : PagedResult
    {
        /// <summary>
        /// Result rows
        /// </summary>
        [JsonPropertyOrder(10)]
        public IList<T> Rows { get; init; }
    }
}
