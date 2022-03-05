using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Expenses
{
    public class DeleteCategoryHandler : Handler<DeleteCategory>
    {
        private readonly ILogger<DeleteCategoryHandler> _logger;
        private readonly IEntityIDFactory _entityIDFactory;
        private readonly ICategoryRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(ILogger<DeleteCategoryHandler> logger,
            IEntityIDFactory entityIDFactory,
            ICategoryRepository repository,
            IUnitOfWork unitOfWork) : base(logger)
        {
            _logger          = logger;
            _entityIDFactory = entityIDFactory;
            _repository      = repository;
            _unitOfWork      = unitOfWork;
        }

        protected override async Task<bool> HandleCore(DeleteCategory request, CancellationToken cancellationToken)
        {
            try
            {
                var categoryID = _entityIDFactory.GetID<CategoryID>(request.ID);
                var category = await _repository.GetByID(categoryID, cancellationToken);
                if (category == null)
                    return await Failure("Entity not found! [ID: {request.Id} - Version: {request.RowVersion}]");
                if (category.RowVersion != request.RowVersion)
                    return await Failure(
                        $"Entity changed by other user/process! [ID: {request.ID} - Version: {request.RowVersion}]");

                _repository.Delete(category);
                await _unitOfWork.SaveAsync(cancellationToken);
                return true;
            }
            // TODO Specify exception
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteCategory failed!");
                return await Failure($"Could not delete category with ID {request.ID}\\r\\n{ex.GetAllMessages()}");
            }
        }
    }
}