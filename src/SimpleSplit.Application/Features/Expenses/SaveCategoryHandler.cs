using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class SaveCategoryHandler : Handler<SaveCategory, CategoryViewModel>
    {
        private readonly IEntityIDFactory _entityIDFactory;
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SaveCategoryHandler> _logger;

        public SaveCategoryHandler(IEntityIDFactory entityIDFactory,
            ICategoryRepository categories,
            IUnitOfWork unitOfWork,
            ILogger<SaveCategoryHandler> logger) : base(logger)
        {
            _entityIDFactory = entityIDFactory;
            _repository      = categories;
            _unitOfWork      = unitOfWork;
            _logger          = logger;
        }

        protected override async Task<CategoryViewModel> HandleCore(SaveCategory request,
            CancellationToken cancellationToken)
        {
            var category = request.Model.IsNew
                ? new Category(_entityIDFactory.NextID<CategoryID>())
                : await _repository.GetByID(new CategoryID(request.Model.ID), cancellationToken);
            if (category.RowVersion != request.Model.RowVersion)
                return await Failure(
                    $"Entity changed by other user/process! [ID: {request.Model.ID} - Request Version: {request.Model.RowVersion}]");

            try
            {
                category.Description = request.Model.Description;
                category.Kind = (Category.CategoryKind)Enum.ToObject(typeof(Category.CategoryKind), request.Model.Kind);

                if (category.IsNew)
                    _repository.Add(category);

                await _unitOfWork.SaveAsync(cancellationToken);
                return category.ToViewModel();
            }
            // TODO Specify exception
            catch (Exception ex)
            {
                _logger.LogError(ex, "Concurrency conflict on category save!");
                return await Failure(ex.GetAllMessages());
            }
        }
    }
}