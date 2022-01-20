using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Expenses
{
    public class DeleteExpense : Request
    {
        public long ID { get; set; }
        public int RowVersion { get; set; }
    }
}
