using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Expenses
{
    public interface IExpenseRepository : IRepository<Expense, ExpenseID>
    {
    }
}
