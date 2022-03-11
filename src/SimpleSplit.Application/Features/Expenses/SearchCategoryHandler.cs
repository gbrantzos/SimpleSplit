using SimpleSplit.Application.Base;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SearchCategoryHandler : Handler<SearchCategories, PagedResult<CategoryViewModel>>
    {
        private readonly ICategoryRepository _repository;
        private readonly ISortingParser _sortingParser;
        private readonly IConditionParser _conditionParser;

        public SearchCategoryHandler(ICategoryRepository repository, ISortingParser sortingParser,
            IConditionParser conditionParser)
        {
            _repository      = repository;
            _sortingParser   = sortingParser;
            _conditionParser = conditionParser;
        }

        protected override async Task<PagedResult<CategoryViewModel>> HandleCore(SearchCategories request,
            CancellationToken cancellationToken)
        {
            var specifications = _conditionParser.BuildSpecifications<Category>(new ConditionGroup
            {
                Grouping = ConditionGroup.GroupingOperator.And,
                Conditions = request.SearchConditions
                    .Select(Condition.FromString)
                    .ToList()
            });
            var sorting = _sortingParser.BuildSorting<Category>(request.SortingDetails);
            var models = await _repository.Find(specifications,
                sorting,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            var totalRows = await _repository.Count(specifications, cancellationToken);

            return new PagedResult<CategoryViewModel>
            {
                CurrentPage = request.PageNumber,
                TotalRows   = totalRows,
                PageSize    = request.PageSize <= 0 ? totalRows : request.PageSize,
                Rows        = models.Select(x => x.ToViewModel()).ToList()
            };
        }
    }
}