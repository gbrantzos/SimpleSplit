using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveExpenseHandler : Handler<SaveExpense, ExpenseViewModel>
    {
        private readonly ILogger<SaveExpenseHandler> _logger;
        private readonly IEntityIDFactory _entityIDFactory;
        private readonly IExpenseRepository _repository;
        private readonly ICategoryRepository _categories;
        private readonly IUnitOfWork _unitOfWork;

        public SaveExpenseHandler(ILogger<SaveExpenseHandler> logger,
            IEntityIDFactory entityIDFactory,
            IExpenseRepository repository,
            ICategoryRepository categories,
            IUnitOfWork unitOfWork) : base(logger)
        {
            _logger = logger;
            _entityIDFactory = entityIDFactory;
            _repository = repository;
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        protected override async Task<ExpenseViewModel> HandleCore(SaveExpense request, CancellationToken cancellationToken)
        {
            var expense = request.Model.IsNew
                ? new Expense(_entityIDFactory.NextID<ExpenseID>())
                : await _repository.GetByID(new ExpenseID(request.Model.ID), cancellationToken);
            if (expense.RowVersion != request.Model.RowVersion)
                return await Failure($"Entity changed by other user/process! [ID: {request.Model.ID} - Request Version: {request.Model.RowVersion}]");

            try
            {
                var categories = (await _categories.GetAll()).ToList();

                expense.Description   = request.Model.Description;
                expense.Amount        = Money.InEuro(request.Model.Amount);
                expense.IsOwnerCharge = request.Model.IsOwnerCharge;
                expense.EnteredAt     = request.Model.EnteredAt;

                var category = categories.FirstOrDefault(c => c.Description.Equals(request.Model.Category, StringComparison.CurrentCultureIgnoreCase));
                if (category != null)
                    expense.Category = category;

                if (expense.IsNew)
                    _repository.Add(expense);

                await _unitOfWork.SaveAsync(cancellationToken);
                return expense.ToViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Concurrency conflict on expense save!");
                return await Failure(ex.GetAllMessages());
            }
        }
    }
}
