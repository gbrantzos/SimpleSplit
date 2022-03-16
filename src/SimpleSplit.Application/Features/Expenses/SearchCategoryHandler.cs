using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SearchCategoryHandler : SearchHandler<SearchCategories, CategoryViewModel>.WithEntityAndID<Category,
        CategoryID>.WithRepository<ICategoryRepository>
    {
        public SearchCategoryHandler(ILogger<SearchCategoryHandler> logger,
            IMapper mapper,
            ICategoryRepository repository,
            ISortingParser sortingParser,
            IConditionParser conditionParser) : base(logger, mapper, repository, sortingParser, conditionParser)
        {
        }
    }
}