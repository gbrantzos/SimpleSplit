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
        public SearchExpensesHandler(ILogger<SearchExpensesHandler> logger,
            IMapper mapper,
            IExpenseRepository repository,
            ISortingParser sortingParser,
            IConditionParser conditionParser) : base(logger, mapper, repository, sortingParser, conditionParser)
        {
        }
    }
}
