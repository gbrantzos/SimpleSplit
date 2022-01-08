using SimpleSplit.Domain.Exceptions;

namespace SimpleSplit.Domain.Base
{
    public interface IRepository<TEntity, in TID>
        where TEntity : Entity<TID>
        where TID : EntityID
    {
        /// <summary>
        /// Get <typeparamref name="TEntity"/> with given <typeparamref name="TID"/>.
        /// Throws <see cref="EntityNotFoundException"/> if
        /// no entity exists with given key.
        /// </summary>
        /// <param name="entityID">The ID of the entity to get</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The Item with given ID</returns>
        Task<TEntity> GetByID(TID entityID, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a new <typeparamref name="TEntity"/> on repository.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns></returns>
        void Add(TEntity entity);

        /// <summary>
        /// Delete an existing <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <returns></returns>
        void Delete(TEntity entity);

        /// <summary>
        /// Search for <typeparamref name="TEntity"/> using specified criteria.
        /// Returns an empty array if criteria are not matched
        /// </summary>
        /// <param name="criteria">Search criteria</param>
        /// <param name="sorting">Sorting information, defaults to null (no sorting).</param>
        /// <param name="details">Entity details to include in results, defaults to null.</param>
        /// <param name="pageNumber">Page number in case of paged request. Defaults to 1.</param>
        /// <param name="pageSize">Page size in case of paged request. Defaults to -1 (na paging).</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> Find(Specification<TEntity> criteria,
            IEnumerable<Sorting<TEntity>> sorting = null,
            string[] details = null,
            int pageNumber = 1,
            int pageSize = -1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the count of a query using specified criteria.
        /// </summary>
        /// <param name="criteria">Search criteria</param>
        /// <returns></returns>
        Task<int> Count(Specification<TEntity> criteria, CancellationToken cancellationToken = default);
    }
}