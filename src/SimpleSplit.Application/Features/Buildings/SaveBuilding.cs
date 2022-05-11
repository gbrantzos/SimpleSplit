using SimpleSplit.Application.Base;
using SimpleSplit.Application.Base.Crud;

namespace SimpleSplit.Application.Features.Buildings
{
    public class SaveBuilding : Request<BuildingViewModel>, ISaveRequest<BuildingViewModel>
    {
        public BuildingViewModel Model { get; init; }
    }
}
