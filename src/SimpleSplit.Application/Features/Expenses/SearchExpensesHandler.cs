using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SearchExpensesHandler : Handler<SearchExpenses, PagedResult<ExpenseViewModel>>
    {
        private readonly IExpenseRepository _repository;
        private readonly ISortingParser _sortingExpressionBuilder;
        private readonly IConditionParser _conditionParser;

        public SearchExpensesHandler(ILogger<SearchExpensesHandler> logger,
            IExpenseRepository repository,
            ISortingParser sortingExpressionBuilder,
            IConditionParser conditionParser) : base(logger)
        {
            _repository = repository;
            _sortingExpressionBuilder = sortingExpressionBuilder;
            _conditionParser = conditionParser;
        }

        protected override async Task<PagedResult<ExpenseViewModel>> HandleCore(SearchExpenses request,
            CancellationToken cancellationToken)
        {
            var specs = _conditionParser.ParseConditions<Expense>(new ConditionGroup
            {
                Grouping = ConditionGroup.GroupingOperator.And,
                Conditions = request.SearchConditions
                    .Select(Condition.FromString)
                    .ToList()
            });
            var sorting = _sortingExpressionBuilder.BuildSorting<Expense>(request.SortingDetails);

            var models = await _repository.Find(specs, sorting,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            var totalRows = await _repository.Count(specs, cancellationToken);

            return new PagedResult<ExpenseViewModel>(request.PageNumber,
                request.PageSize,
                totalRows,
                models.Select(x => x.ToViewModel()).ToList());
        }
    }
}
