using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveExpenseHandler : SaveHandler<SaveExpense, ExpenseViewModel>
        .WithEntityAndID<Expense, ExpenseID>
        .WithRepository<IExpenseRepository>
    {
        private readonly ICategoryRepository _categoryRepository;

        public SaveExpenseHandler(IExpenseRepository repository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            IEntityIDFactory entityIDFactory,
            IMapper mapper,
            ILogger<SaveExpenseHandler> logger) : base(repository, unitOfWork, entityIDFactory, mapper, logger)
            => _categoryRepository = categoryRepository;

        protected override async Task ApplyChanges(SaveExpense request, Expense expense)
        {
            var categories = (await _categoryRepository.GetAll()).ToList();
            expense.Description   = request.Model.Description;
            expense.Amount        = Money.InEuro(request.Model.Amount);
            expense.IsOwnerCharge = request.Model.IsOwnerCharge;
            expense.EnteredAt     = request.Model.EnteredAt;
            if (request.Model.Category != null)
            {
                var category = categories.FirstOrDefault(c =>
                    c.Description.Equals(request.Model.Category, StringComparison.CurrentCultureIgnoreCase));
                if (category != null)
                    expense.Category = category;
            }
            else
            {
                expense.Category = null;
            }
        }
    }
}