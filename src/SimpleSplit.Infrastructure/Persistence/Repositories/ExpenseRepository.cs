using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;
using SimpleSplit.Infrastructure.Persistence.Base;

namespace SimpleSplit.Infrastructure.Persistence.Repositories
{
    public class ExpenseRepository : GenericRepository<Expense, ExpenseID>, IExpenseRepository
    {
        public ExpenseRepository(SimpleSplitDbContext dbContext) : base(dbContext)
        {
        }
    }
}
