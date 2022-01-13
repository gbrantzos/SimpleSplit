using System.ComponentModel;
using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    [DisplayName("Expenses")]
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

        /// <summary>
        /// Category description.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Expense is charged on owner of appartment
        /// </summary>
        public bool IsOwnerCharge { get; set; }

        /// <summary>
        /// When this expense was shared
        /// </summary>
        public DateTime? SharedAt { get; set; }
    }

    public static class ExpenseViewModelExtensions
    {
        public static ExpenseViewModel ToViewModel(this Expense domainObject)
        {
            return new ExpenseViewModel
            {
                ID            = domainObject.ID.Value,
                RowVersion    = domainObject.RowVersion,
                Description   = domainObject.Description,
                Amount        = domainObject.Amount.Amount,
                EnteredAt     = domainObject.EnteredAt,
                Category      = domainObject.Category?.Description,
                IsOwnerCharge = domainObject.IsOwnerCharge,
                SharedAt      = domainObject.SharedAt,
            };
        }
    }
}