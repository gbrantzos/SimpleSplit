using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SearchExpenses : PagedRequest<ExpenseViewModel>
    {
        public IEnumerable<string> SearchConditions { get; set; }
    }
}
