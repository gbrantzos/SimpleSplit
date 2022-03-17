using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveCategoryHandler : SaveHandler<SaveCategory, CategoryViewModel>
        .WithEntityAndID<Category, CategoryID>
        .WithRepository<ICategoryRepository>
    {
        public SaveCategoryHandler(ICategoryRepository repository,
            IUnitOfWork unitOfWork,
            IEntityIDFactory entityIDFactory,
            IMapper mapper,
            ILogger<SaveCategoryHandler> logger) : base(repository, unitOfWork, entityIDFactory, mapper, logger)
        {
        }

        protected override Task ApplyChanges(SaveCategory request, Category entity)
        {
            entity.Description = request.Model.Description;
            entity.Kind = (Category.CategoryKind)Enum.ToObject(typeof(Category.CategoryKind), request.Model.Kind);
            
            return Task.CompletedTask;
        }
    }
}