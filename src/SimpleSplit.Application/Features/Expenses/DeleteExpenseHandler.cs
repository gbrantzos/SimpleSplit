using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class DeleteExpenseHandler : DeleteHandler<DeleteExpense>
        .WithEntityAndID<Expense, ExpenseID>
        .WithRepository<IExpenseRepository>
    {
        public DeleteExpenseHandler(IExpenseRepository repository,
            IUnitOfWork unitOfWork,
            IEntityIDFactory entityIDFactory,
            ILogger<DeleteExpenseHandler> logger) : base(repository, unitOfWork, entityIDFactory, logger)
        {
        }
    }
}