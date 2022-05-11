using Mapster;
using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Buildings;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Buildings
{
    public class BuildingViewModel : ViewModel
    {
        public string Description { get; set; }
        public string AddressStreet { get; set; }
        public string AddressNumber { get; set; }
        public string AddressZipCode { get; set; }
        public string AddressCity { get; set; }
        public List<ApartmentViewModel> Apartments { get; set; }
    }

    public class ApartmentViewModel : ViewModel
    {
        public string Code { get; set; }
        public string SortingNum { get; set; }
        public string Owner { get; set; }
        public string Dweller { get; set; }

        public double HeatingRatio { get; set; }
        public double ElevatorRatio { get; set; }
        public double SharedRatio { get; set; }
    }

    public class BuildingViewModelMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Building, BuildingViewModel>()
                .Map(dest => dest.ID, src => src.ID.Value);
            config.NewConfig<Apartment, ApartmentViewModel>()
                .Map(dest => dest.ID, src => src.ID.Value)
                .Map(dest => dest.HeatingRatio, src => Math.Round(src.Ratios[Category.CategoryKind.Heating] * 1000, 2))
                .Map(dest => dest.ElevatorRatio, src => Math.Round(src.Ratios[Category.CategoryKind.Elevator] * 1000, 2))
                .Map(dest => dest.SharedRatio, src => Math.Round(src.Ratios[Category.CategoryKind.Shared] * 1000, 2));
        }
    }
}
