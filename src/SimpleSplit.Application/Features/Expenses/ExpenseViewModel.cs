using System.ComponentModel;
using Mapster;
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
        /// Category ID.
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// Expense is charged on owner of apartment
        /// </summary>
        public bool IsOwnerCharge { get; set; }

        /// <summary>
        /// When this expense was shared
        /// </summary>
        public DateTime? SharedAt { get; set; }
    }

    public class ExpenseViewModelMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Expense, ExpenseViewModel>()
                .Map(dest => dest.ID, src => src.ID.Value)
                .Map(dest => dest.Amount, src => src.Amount.Amount)
                .Map(dest => dest.Category, src => src.Category == null ? null : src.Category.Description)
                .Map(dest => dest.CategoryId, src => src.Category == null ? 0 : src.Category.ID.Value);
        }
    }
}