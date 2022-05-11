using Microsoft.Extensions.Logging;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Base.Exceptions;

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
                        var entity = await Repository.GetByID(entityID, cancellationToken);
                        if (entity == null)
                            return await Failure(
                                "Entity not found! [ID: {request.Id} - Version: {request.RowVersion}]");
                        if (entity.RowVersion != request.RowVersion)
                            throw new ConcurrencyConflictException(entity, entity.ID, request.RowVersion);

                        Repository.Delete(entity);
                        await UnitOfWork.SaveAsync(cancellationToken);
                        return true;
                    }
                    catch (ConcurrencyConflictException cx)
                    {
                        Logger.LogError(cx, $"Concurrency conflict on {typeof(TEntity).Name} delete!");
                        return await Failure(cx.Message);
                    }
                }
            }
        }
    }
}
