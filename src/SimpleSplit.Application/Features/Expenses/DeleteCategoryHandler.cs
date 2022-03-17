using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class DeleteCategoryHandler : DeleteHandler<DeleteCategory>
        .WithEntityAndID<Category, CategoryID>
        .WithRepository<ICategoryRepository>
    {
        public DeleteCategoryHandler(ICategoryRepository repository,
            IUnitOfWork unitOfWork,
            IEntityIDFactory entityIDFactory,
            ILogger<DeleteCategoryHandler> logger) : base(repository, unitOfWork, entityIDFactory, logger)
        {
        }
    }
}