using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Application.Base.Crud
{
    // Ideas from https://tyrrrz.me/blog/fluent-generics
    public static class SearchHandler<TRequest, TResult>
        where TRequest : Request<PagedResult<TResult>>, IPagedRequest
        where TResult : ViewModel
    {
        public static class WithEntityAndID<TEntity, TEntityID>
            where TEntity : Entity<TEntityID>
            where TEntityID : EntityID
        {
            public abstract class WithRepository<TRepository> : Handler<TRequest, PagedResult<TResult>>
                where TRepository : IRepository<TEntity, TEntityID>
            {
                protected readonly TRepository Repository;
                protected readonly ISortingParser SortingParser;
                protected readonly IConditionParser ConditionParser;
                protected readonly IMapper Mapper;

                protected WithRepository(TRepository repository,
                    ISortingParser sortingParser,
                    IConditionParser conditionParser,
                    IMapper mapper,
                    ILogger logger) : base(logger)
                {
                    Repository      = repository;
                    SortingParser   = sortingParser;
                    ConditionParser = conditionParser;
                    Mapper          = mapper;
                }

                protected override async Task<PagedResult<TResult>> HandleCore(TRequest request,
                    CancellationToken cancellationToken)
                {
                    var conditions = request.AdvancedSearch ?? new ConditionGroup
                    {
                        Grouping = ConditionGroup.GroupingOperator.And,
                        Conditions = request.SearchConditions
                            .Select(Condition.FromString)
                            .ToList()
                    };
                    
                    var specifications = ConditionParser.BuildSpecifications<TEntity>(conditions);
                    var sorting = SortingParser.BuildSorting<TEntity>(request.SortingDetails);

                    var models = await Repository.Find(specifications, sorting,
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        cancellationToken: cancellationToken);
                    var totalRows = await Repository.Count(specifications, cancellationToken);

                    return new PagedResult<TResult>
                    {
                        CurrentPage = request.PageNumber,
                        TotalRows   = totalRows,
                        PageSize    = request.PageSize <= 0 ? totalRows : request.PageSize,
                        Rows        = models.Select(r => Mapper.Map<TResult>(r)).ToList()
                    };
                }
            }
        }
    }
}