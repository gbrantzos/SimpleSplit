using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Features.Buildings;

namespace SimpleSplit.Application.Features.Buildings
{
    public class DeleteBuildingHandler : DeleteHandler<DeleteBuilding>
        .WithEntityAndID<Building, BuildingID>
        .WithRepository<IBuildingRepository>
    {
        public DeleteBuildingHandler(IBuildingRepository repository,
            IUnitOfWork unitOfWork,
            IEntityIDFactory entityIDFactory,
            ILogger<DeleteBuildingHandler> logger) : base(repository, unitOfWork, entityIDFactory, logger)
        {
        }
    }
}
