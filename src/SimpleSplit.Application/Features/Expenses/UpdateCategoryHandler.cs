using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class UpdateCategoryHandler : Handler<UpdateCategory>
    {
        private readonly IExpenseRepository  _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IEntityIDFactory    _entityIDFactory;
        private readonly IUnitOfWork         _unitOfWork;

        public UpdateCategoryHandler(IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository,
            IEntityIDFactory entityIDFactory,
            IUnitOfWork unitOfWork)
        {
            _expenseRepository  = expenseRepository;
            _categoryRepository = categoryRepository;
            _entityIDFactory    = entityIDFactory;
            _unitOfWork         = unitOfWork;
        }

        protected override async Task<bool> HandleCore(UpdateCategory request, CancellationToken cancellationToken)
        {
            var categoryID = _entityIDFactory.GetID<CategoryID>(request.CategoryID);
            var category = await _categoryRepository.GetByID(categoryID, cancellationToken);
            
            foreach (var expenseViewModel in request.Expenses)
            {
                var expenseID = _entityIDFactory.GetID<ExpenseID>(expenseViewModel.ID);
                var expense = await _expenseRepository.GetByID(expenseID, cancellationToken);
                expense.Category = category;
            }

            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}