using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleSplit.Common;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Exceptions;

namespace SimpleSplit.Infrastructure.Persistence.Base
{
    /// <summary>
    /// Base repository for all entities
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class GenericRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
        where TKey : EntityID
    {
        protected readonly string RepositoryName;
        protected readonly SimpleSplitDbContext DbContext;
        protected readonly DbSet<TEntity> Set;

        protected GenericRepository(SimpleSplitDbContext dbContext)
        {
            DbContext      = dbContext.ThrowIfNull(nameof(dbContext));
            Set            = DbContext.Set<TEntity>();
            RepositoryName = GetType().Name;
        }

        /// <summary>
        /// Entity navigation properties to include by default.
        /// </summary>
        protected virtual string[] DefaultInclude => Array.Empty<string>();

        /// <summary>
        /// Default sorting expression.
        /// </summary>
        protected virtual Expression<Func<TEntity, object>> DefaultSort => entity => entity.ID;

        public virtual async Task<TEntity> GetByID(TKey id, CancellationToken cancellationToken = default)
        {
            var dbSet = Set;
            var query = DefaultInclude.Length == 0
                ? dbSet
                : DbSetWithDetails(dbSet, DefaultInclude);
            var entity = await query
                .TagWith($"{RepositoryName} :: {nameof(GetByID)}")
                .SingleOrDefaultAsync(entity => entity.ID == id, cancellationToken);
            if (entity is null)
                throw new EntityNotFoundException(typeof(TEntity), id);

            return entity;
        }

        public virtual void Add(TEntity entity) => Set.Add(entity);

        public virtual void Delete(TEntity entity) => Set.Remove(entity);

        public async Task<int> Count(Specification<TEntity> criteria,
            CancellationToken cancellationToken = default)
        {
            return await Set
                .Where(criteria.ToExpression())
                .TagWith($"{RepositoryName} :: Count")
                .CountAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> Find(Specification<TEntity> criteria,
            IEnumerable<Sorting<TEntity>> sorting = null,
            string[] details = null,
            int pageNumber = 1,
            int pageSize = -1,
            CancellationToken cancellationToken = default)
        {
            var dbSet = Set;
            var query = details?.Length == 0
                ? dbSet
                : DbSetWithDetails(dbSet, details);
            query = query.Where(criteria.ToExpression());
            var sortedQuery = AddSorting(query, sorting);

            var pagedRequest = pageNumber >= 1 && pageSize >= 0;
            return pagedRequest
                ? await PagedResults(sortedQuery, pageNumber, pageSize, cancellationToken)
                : await FullResults(sortedQuery, cancellationToken);
        }

        private async Task<IEnumerable<TEntity>> FullResults(IQueryable<TEntity> query,
            CancellationToken cancellationToken)
        {
            return await query
                .TagWith($"{RepositoryName} :: {nameof(FullResults)}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        private async Task<IEnumerable<TEntity>> PagedResults(IQueryable<TEntity> query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var pageOffset = (pageNumber - 1) * pageSize;
            return await query
                .TagWith($"{RepositoryName} :: {nameof(PagedResults)}")
                .Skip(pageOffset)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        private IOrderedQueryable<TEntity> AddSorting(IQueryable<TEntity> query, IEnumerable<Sorting<TEntity>> sorting)
        {
            var sortingList = sorting?.ToList();
            if (sortingList == null || sortingList?.Count == 0)
                return query.OrderBy(DefaultSort);

            var first = true;
            IOrderedQueryable<TEntity> sortedQuery = null;
            foreach (var sort in sortingList)
            {
                switch (sort.Direction)
                {
                    case Sorting.SortDirection.Ascending:
                        sortedQuery = first
                            ? query.OrderBy(sort.Expression)
                            : sortedQuery.ThenBy(sort.Expression);
                        first = false;
                        break;
                    default:
                        sortedQuery = first
                            ? query.OrderByDescending(sort.Expression)
                            : sortedQuery.ThenByDescending(sort.Expression);
                        first = false;
                        break;
                }
            }

            return sortedQuery;
        }

        private static IQueryable<TEntity> DbSetWithDetails(IQueryable<TEntity> query, IEnumerable<string> details)
        {
            foreach (var detail in details ?? Array.Empty<string>())
                query = query.Include(detail);

            return query;
        }
    }
}