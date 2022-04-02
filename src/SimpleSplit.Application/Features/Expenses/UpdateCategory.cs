using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Expenses
{
    public class UpdateCategory : Request
    {
        public ExpenseViewModel[] Expenses { get; set; }
        public long CategoryID { get; set; }
    }
}