using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Application.Services;
using SimpleSplit.Domain.Features.Buildings;

namespace SimpleSplit.Application.Features.Buildings
{
    public class SearchBuildingsHandler : SearchHandler<SearchBuildings, BuildingViewModel>
        .WithEntityAndID<Building, BuildingID>
        .WithRepository<IBuildingRepository>
    {
        public SearchBuildingsHandler(IBuildingRepository repository,
            ISortingParser sortingParser,
            IConditionParser conditionParser,
            IMapper mapper,
            ILogger<SearchBuildingsHandler> logger) : base(repository, sortingParser, conditionParser, mapper, logger)
        {
        }
    }
}
