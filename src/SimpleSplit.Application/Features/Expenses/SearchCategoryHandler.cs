using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SearchCategoryHandler : Handler<SearchCategories, PagedResult<CategoryViewModel>>
    {
        private readonly ICategoryRepository _repository;

        public SearchCategoryHandler(ICategoryRepository repository)
            => _repository = repository;

        protected override async Task<PagedResult<CategoryViewModel>> HandleCore(SearchCategories request,
            CancellationToken cancellationToken)
        {
            var specifications = Specification<Category>.True;
            var models = await _repository.Find(specifications,
                null,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            var totalRows = await _repository.Count(specifications, cancellationToken);
            
            return new PagedResult<CategoryViewModel>
            {
                CurrentPage = request.PageNumber,
                TotalRows   = totalRows,
                PageSize    = request.PageSize,
                Rows        = models.Select(x => x.ToViewModel()).ToList()
            };
        }
    }
}