using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Expenses
{
    public class GetExpense : Request<ExpenseViewModel>
    {
        /// <summary>
        /// Item ID to search for.
        /// </summary>
        public long ID { get; init; }
    }
}
