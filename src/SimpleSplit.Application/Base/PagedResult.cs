using System.Text.Json.Serialization;

namespace SimpleSplit.Application.Base
{
    public abstract class PagedResult
    {
        /// <summary>
        /// Current page
        /// </summary>
        public int CurrentPage { get; protected set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; protected set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; protected set; }

        /// <summary>
        /// Total number of rows
        /// </summary>
        public int TotalRows { get; protected set; }

        protected PagedResult(int currentPage, int pageSize, int totalRows)
        {
            CurrentPage = currentPage;
            PageSize    = pageSize;
            TotalRows   = totalRows;
            TotalPages  = pageSize == -1 ? 1 : (int)Math.Ceiling((double)totalRows / pageSize);
        }
    }

    public class PagedResult<T> : PagedResult
    {
        /// <summary>
        /// Result rows
        /// </summary>
        [JsonPropertyOrder(10)]
        public IList<T> Rows { get; }

        public PagedResult(int currentPage, int pageSize, int totalRows, IList<T> rows)
            : base(currentPage, pageSize, totalRows) => Rows = rows;
    }
}
