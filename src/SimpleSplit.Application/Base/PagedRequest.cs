namespace SimpleSplit.Application.Base
{
    public class PagedRequest<T> : Request<PagedResult<T>>
    {
        public IEnumerable<string> SortingDetails { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = -1;
    }
}
