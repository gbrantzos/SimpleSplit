using FluentValidation;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveExpenseValidator : AbstractValidator<SaveExpense>
    {
        public SaveExpenseValidator()
        {
            RuleFor(m => m.Model).NotNull();
            RuleFor(m => m.Model.Description)
                .NotEmpty()
                .WithMessage($"{nameof(SaveExpense.Model.Description)} cannot be null or empty");
            RuleFor(m => m.Model.Amount)
                .GreaterThan(0)
                .WithMessage($"{nameof(SaveExpense.Model.Amount)} must be greater than zero");
        }
    }
}
