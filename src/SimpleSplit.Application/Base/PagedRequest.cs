using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Application.Services;

namespace SimpleSplit.Application.Base
{
    public class PagedRequest<T> : Request<PagedResult<T>>, IPagedRequest
    {
        public IEnumerable<string> SearchConditions { get; set; }
        public IEnumerable<string> SortingDetails { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = -1;
        
        public ConditionGroup AdvancedSearch { get; set; }
    }
}
