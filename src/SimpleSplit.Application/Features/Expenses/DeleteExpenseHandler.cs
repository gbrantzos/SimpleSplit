using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class DeleteExpenseHandler : Handler<DeleteExpense>
    {
        private readonly ILogger<DeleteExpenseHandler> _logger;
        private readonly IEntityIDFactory _entityIDFactory;
        private readonly IExpenseRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteExpenseHandler(ILogger<DeleteExpenseHandler> logger,
            IEntityIDFactory entityIDFactory,
            IExpenseRepository repository,
            IUnitOfWork unitOfWork) : base(logger)
        {
            _logger = logger;
            _entityIDFactory = entityIDFactory;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        
        protected override async Task<bool> HandleCore(DeleteExpense request, CancellationToken cancellationToken)
        {
            try
            {
                var expenseID = _entityIDFactory.GetID<ExpenseID>(request.ID);
                var expense = await _repository.GetByID(expenseID, cancellationToken);
                if (expense == null)
                    return await Failure("Entity not found! [ID: {request.Id} - Version: {request.RowVersion}]");
                if (expense.RowVersion != request.RowVersion)
                    return await Failure($"Entity changed by other user/process! [ID: {request.ID} - Version: {request.RowVersion}]");

                _repository.Delete(expense);
                await _unitOfWork.SaveAsync(cancellationToken);
                return true;
            }
            // TODO Specify exception
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteExpense failed!");
                return await Failure($"Could not delete expense with ID {request.ID}\\r\\n{ex.GetAllMessages()}");
            }
        }
    }
}
