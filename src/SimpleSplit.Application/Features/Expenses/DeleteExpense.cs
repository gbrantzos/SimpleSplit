using SimpleSplit.Application.Base;
using SimpleSplit.Application.Base.Crud;

namespace SimpleSplit.Application.Features.Expenses
{
    public class DeleteExpense : Request, IDeleteRequest
    {
        public long ID { get; set; }
        public int RowVersion { get; set; }
    }
}
