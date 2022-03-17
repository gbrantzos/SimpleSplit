using SimpleSplit.Application.Base;
using SimpleSplit.Application.Base.Crud;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveExpense : Request<ExpenseViewModel>, ISaveRequest<ExpenseViewModel>
    {
        public ExpenseViewModel Model { get; init; }
    }
}
