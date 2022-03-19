using SimpleSplit.Application.Services;

namespace SimpleSplit.Application.Base.Crud
{
    public interface IPagedRequest
    {
        /// <summary>
        /// Search conditions as received from presentation layer, i.e. "description|starts|Test"
        /// </summary>
        IEnumerable<string> SearchConditions { get; }
                
        /// <summary>
        /// Sorting, as array of properties prefixed by +|- to define ordering direction 
        /// </summary>
        IEnumerable<string> SortingDetails { get; }
                
        /// <summary>
        /// Page number to retrieve
        /// </summary>
        int PageNumber { get; }
                
        /// <summary>
        /// Page size for results. 0 (or lower) means no paging!
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Conditions group, for advanced search 
        /// </summary>
        ConditionGroup AdvancedSearch { get; }
    }
}