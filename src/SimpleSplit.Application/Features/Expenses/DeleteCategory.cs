using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Features.Expenses
{
    public class DeleteCategory : Request
    {
        public long ID { get; set; }
        public int RowVersion { get; set; }   
    }
}