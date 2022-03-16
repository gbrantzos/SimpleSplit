using Mapster;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Exceptions;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class GetExpenseHandler : Handler<GetExpense, ExpenseViewModel>
    {
        private readonly ILogger<GetExpenseHandler> _logger;
        private readonly IExpenseRepository _repository;

        public GetExpenseHandler(ILogger<GetExpenseHandler> logger, IExpenseRepository repository) : base(logger)
        {
            _logger = logger;
            _repository = repository;
        }

        protected override async Task<ExpenseViewModel> HandleCore(GetExpense request,
            CancellationToken cancellationToken)
        {
            try
            {
                var expenseID = new ExpenseID(request.ID);
                var expense = await _repository.GetByID(expenseID, cancellationToken);

                return expense.Adapt<ExpenseViewModel>();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogWarning(ex, $"{nameof(GetExpenseHandler)} failure!");
                return await Failure(ex.Message);
            }
        }
    }
}
