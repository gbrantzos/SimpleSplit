using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Base.Exceptions;

namespace SimpleSplit.Application.Base.Crud
{
    public static class SaveHandler<TRequest, TResult>
        where TRequest : Request<TResult>, ISaveRequest<TResult>
        where TResult : ViewModel
    {
        public static class WithEntityAndID<TEntity, TEntityID>
            where TEntity : Entity<TEntityID>
            where TEntityID : EntityID
        {
            public abstract class WithRepository<TRepository> : Handler<TRequest, TResult>
                where TRepository : IRepository<TEntity, TEntityID>
            {
                protected readonly TRepository Repository;
                protected readonly IUnitOfWork UnitOfWork;
                protected readonly IEntityIDFactory EntityIDFactory;
                protected readonly IMapper Mapper;
                protected readonly ILogger Logger;

                protected WithRepository(TRepository repository,
                    IUnitOfWork unitOfWork,
                    IEntityIDFactory entityIDFactory,
                    IMapper mapper,
                    ILogger logger) : base(logger)
                {
                    Logger          = logger;
                    Repository      = repository;
                    Mapper          = mapper;
                    UnitOfWork      = unitOfWork;
                    EntityIDFactory = entityIDFactory;
                }

                protected override async Task<TResult> HandleCore(TRequest request, CancellationToken cancellationToken)
                {
                    var entity = request.Model.IsNew
                        ? (TEntity) InstanceFactory.CreateInstance(typeof(TEntity), EntityIDFactory.NextID<TEntityID>())
                        : await Repository.GetByID((TEntityID) InstanceFactory.CreateInstance(
                                typeof(TEntityID),
                                request.Model.ID),
                            cancellationToken);
                    if (entity.RowVersion != request.Model.RowVersion)
                        throw new ConcurrencyConflictException(entity, entity.ID, request.Model.RowVersion);

                    try
                    {
                        await ApplyChanges(request, entity);
                        if (entity.IsNew)
                            Repository.Add(entity);

                        await UnitOfWork.SaveAsync(cancellationToken);
                        return Mapper.Map<TResult>(entity);
                    }
                    catch (ConcurrencyConflictException cx)
                    {
                        Logger.LogError(cx, $"Concurrency conflict on {typeof(TEntity).Name} save!");
                        return await Failure(cx.Message);
                    }
                }

                /// <summary>
                /// The method that "updates" the entity (Domain Object) with values from the presentation layer.
                /// </summary>
                /// <param name="request">The full request</param>
                /// <param name="entity">The entity, created or fetched from storage</param>
                protected abstract Task ApplyChanges(TRequest request, TEntity entity);
            }
        }
    }
}
