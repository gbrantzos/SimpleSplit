namespace SimpleSplit.Application.Base.Crud
{
    public interface IPagedRequest
    {
        IEnumerable<string> SearchConditions { get; set; }
        IEnumerable<string> SortingDetails { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
}