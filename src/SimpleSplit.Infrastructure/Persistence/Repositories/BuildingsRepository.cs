using SimpleSplit.Domain.Features.Buildings;
using SimpleSplit.Infrastructure.Persistence.Base;

namespace SimpleSplit.Infrastructure.Persistence.Repositories
{
    public class BuildingsRepository : GenericRepository<Building, BuildingID>, IBuildingRepository
    {
        public BuildingsRepository(SimpleSplitDbContext dbContext) : base(dbContext)
        {
        }

        protected override string[] DefaultInclude => new[] { nameof(Building.Address), nameof(Building.Apartments) };
    }
}