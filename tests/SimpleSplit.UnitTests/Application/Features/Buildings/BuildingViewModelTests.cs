using Mapster;
using SimpleSplit.Application.Features.Buildings;
using SimpleSplit.Domain.Features.Buildings;
using Xunit;

namespace SimpleSplit.UnitTests.Application.Features.Buildings
{
    public class BuildingViewModelTests
    {
        [Fact]
        public void Mapping_DomainObject_To_ViewModel()
        {
            TypeAdapterConfig.GlobalSettings.Scan(typeof(BuildingViewModel).Assembly);
            
            var building = Building.ForTests;
            var viewModel = building.Adapt<BuildingViewModel>();
        }
    }
}