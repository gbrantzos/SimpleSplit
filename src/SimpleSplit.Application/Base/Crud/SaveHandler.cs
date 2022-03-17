using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;

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
                        ? (TEntity)InstanceFactory.CreateInstance(typeof(TEntity), EntityIDFactory.NextID<TEntityID>())
                        : await Repository.GetByID((TEntityID)InstanceFactory.CreateInstance(
                                typeof(TEntityID),
                                request.Model.ID),
                            cancellationToken);
                    if (entity.RowVersion != request.Model.RowVersion)
                        return await Failure(
                            $"Entity changed by other user/process! [ID: {request.Model.ID} - Request Version: {request.Model.RowVersion}]");
                    try
                    {
                        await ApplyChanges(request, entity);
                        if (entity.IsNew)
                            Repository.Add(entity);
                        
                        await UnitOfWork.SaveAsync(cancellationToken);
                        return Mapper.Map<TResult>(entity);
                    }
                    // TODO Specify exception
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Concurrency conflict on category save!");
                        return await Failure(ex.GetAllMessages());
                    }
                }

                /// <summary>
                /// The method that "updates" the enttty (Domain Object) with values from the presentation layer.
                /// </summary>
                /// <param name="request">The full request</param>
                /// <param name="entity">The entity, created or fetched from storage</param>
                protected abstract Task ApplyChanges(TRequest request, TEntity entity);
            }
        }
    }
}