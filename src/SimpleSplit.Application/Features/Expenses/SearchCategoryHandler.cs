using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SearchCategoryHandler : SearchHandler<SearchCategories, CategoryViewModel>
        .WithEntityAndID<Category, CategoryID>
        .WithRepository<ICategoryRepository>
    {
        public SearchCategoryHandler(ICategoryRepository repository,
            IMapper mapper,
            ISortingParser sortingParser,
            IConditionParser conditionParser,
            ILogger<SearchCategoryHandler> logger) : base(repository, sortingParser, conditionParser, mapper, logger)
        {
        }
    }
}