using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SearchExpensesHandler : Handler<SearchExpenses, PagedResult<ExpenseViewModel>>
    {
        private readonly IExpenseRepository _repository;
        private readonly ISortingParser _sortingParser;
        private readonly IConditionParser _conditionParser;

        public SearchExpensesHandler(ILogger<SearchExpensesHandler> logger,
            IExpenseRepository repository,
            ISortingParser sortingParser,
            IConditionParser conditionParser) : base(logger)
        {
            _repository = repository;
            _sortingParser = sortingParser;
            _conditionParser = conditionParser;
        }

        protected override async Task<PagedResult<ExpenseViewModel>> HandleCore(SearchExpenses request,
            CancellationToken cancellationToken)
        {
            var specifications = _conditionParser.BuildSpecifications<Expense>(new ConditionGroup
            {
                Grouping = ConditionGroup.GroupingOperator.And,
                Conditions = request.SearchConditions
                    .Select(Condition.FromString)
                    .ToList()
            });
            var sorting = _sortingParser.BuildSorting<Expense>(request.SortingDetails);
            await Task.Delay(2000);

            var models = await _repository.Find(specifications, sorting,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            var totalRows = await _repository.Count(specifications, cancellationToken);
            return new PagedResult<ExpenseViewModel>
            {
                CurrentPage = request.PageNumber,
                TotalRows   = totalRows,
                PageSize    = request.PageSize,
                Rows        = models.Select(x => x.ToViewModel()).ToList()
            };
        }
    }
}
