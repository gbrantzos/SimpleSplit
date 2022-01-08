using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class ExpenseViewModel : ViewModel
    {
        /// <summary>
        /// Expense description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Entry date
        /// </summary>
        public DateTime EnteredAt { get; set; }
        /// <summary>
        /// Amount in EURO
        /// </summary>
        public decimal Amount { get; set; }
    }

    public static class ExpenseViewModelExtensions
    {
        public static ExpenseViewModel ToViewModel(this Expense domainObject)
        {
            return new ExpenseViewModel
            {
                ID          = domainObject.ID.IDValue,
                RowVersion  = domainObject.RowVersion,
                Description = domainObject.Description,
                Amount      = domainObject.Amount.Amount,
                EnteredAt   = domainObject.EnteredAt,
            };
        }
    }
}