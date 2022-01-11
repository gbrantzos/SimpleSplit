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

        public SearchExpensesHandler(IExpenseRepository repository, ISortingParser sortingExpressionBuilder)
        {
            _repository = repository;
            _sortingExpressionBuilder = sortingExpressionBuilder;
        }

        protected override async Task<PagedResult<ExpenseViewModel>> HandleCore(SearchExpenses request,
            CancellationToken cancellationToken)
        {
            var models = await _repository.Find(Specification<Expense>.True,
                sorting: _sortingExpressionBuilder.BuildSorting<Expense>(request.SortingDetails),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            var totalRows = await _repository.Count(Specification<Expense>.True, cancellationToken);

            return new PagedResult<ExpenseViewModel>(request.PageNumber,
                request.PageSize,
                totalRows,
                models.Select(x => x.ToViewModel()).ToList());
        }
    }
}
