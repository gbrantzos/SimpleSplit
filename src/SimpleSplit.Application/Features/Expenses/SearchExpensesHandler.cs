using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SearchExpensesHandler : SearchHandler<SearchExpenses, ExpenseViewModel>
        .WithEntityAndID<Expense, ExpenseID>
        .WithRepository<IExpenseRepository>
    {
        public SearchExpensesHandler(IExpenseRepository repository,
            ISortingParser sortingParser,
            IConditionParser conditionParser,
            IMapper mapper,
            ILogger<SearchExpensesHandler> logger) : base(repository, sortingParser, conditionParser, mapper, logger)
        {
        }
    }
}