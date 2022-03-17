using Microsoft.Extensions.Logging;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Base.Crud
{
    public static class DeleteHandler<TRequest>
        where TRequest : Request<bool>, IDeleteRequest
    {
        public static class WithEntityAndID<TEntity, TEntityID>
            where TEntity : Entity<TEntityID>
            where TEntityID : EntityID
        {
            public abstract class WithRepository<TRepository> : Handler<TRequest, bool>
                where TRepository : IRepository<TEntity, TEntityID>
            {
                protected readonly TRepository Repository;
                protected readonly IUnitOfWork UnitOfWork;
                protected readonly IEntityIDFactory EntityIDFactory;
                protected readonly ILogger Logger;

                protected WithRepository(TRepository repository,
                    IUnitOfWork unitOfWork,
                    IEntityIDFactory entityIDFactory,
                    ILogger logger)
                    : base(logger)
                {
                    Repository      = repository;
                    UnitOfWork      = unitOfWork;
                    EntityIDFactory = entityIDFactory;
                    Logger          = logger;
                }

                protected override async Task<bool> HandleCore(TRequest request, CancellationToken cancellationToken)
                {
                    try
                    {
                        var entityID = EntityIDFactory.GetID<TEntityID>(request.ID);
                        var expense = await Repository.GetByID(entityID, cancellationToken);
                        if (expense == null)
                            return await Failure(
                                "Entity not found! [ID: {request.Id} - Version: {request.RowVersion}]");
                        if (expense.RowVersion != request.RowVersion)
                            return await Failure(
                                $"Entity changed by other user/process! [ID: {request.ID} - Version: {request.RowVersion}]");

                        Repository.Delete(expense);
                        await UnitOfWork.SaveAsync(cancellationToken);
                        return true;
                    }
                    // TODO Specify exception
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "DeleteExpense failed!");
                        return await Failure(
                            $"Could not delete expense with ID {request.ID}\\r\\n{ex.GetAllMessages()}");
                    }
                }
            }
        }
    }
}