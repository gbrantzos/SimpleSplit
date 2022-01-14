using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveExpense : Request<ExpenseViewModel>
    {
        public ExpenseViewModel Model { get; init; }
    }
}
