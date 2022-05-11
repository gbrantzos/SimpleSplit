using MapsterMapper;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base.Crud;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Base.Exceptions;
using SimpleSplit.Domain.Features.Buildings;
using SimpleSplit.Domain.Features.Expenses;

namespace SimpleSplit.Application.Features.Buildings
{
    public class SaveBuildingHandler : SaveHandler<SaveBuilding, BuildingViewModel>
        .WithEntityAndID<Building, BuildingID>
        .WithRepository<IBuildingRepository>
    {
        public SaveBuildingHandler(IBuildingRepository repository,
            IUnitOfWork unitOfWork,
            IEntityIDFactory entityIDFactory,
            IMapper mapper,
            ILogger<SaveBuildingHandler> logger) : base(repository, unitOfWork, entityIDFactory, mapper, logger)
        {
        }

        protected override Task ApplyChanges(SaveBuilding request, Building building)
        {
            building.Description     = request.Model.Description;
            building.Address.Street  = request.Model.AddressStreet;
            building.Address.Number  = request.Model.AddressNumber;
            building.Address.City    = request.Model.AddressCity;
            building.Address.ZipCode = request.Model.AddressZipCode;

            // Detect deleted items
            var requestIDs = request
                .Model
                .Apartments
                .Where(ap => !ap.IsNew)
                .Select(ap => ap.ID)
                .ToList();
            var missing = building
                .Apartments
                .Where(ap => !requestIDs.Contains(ap.ID.Value))
                .ToList();
            foreach (var apartment in missing)
            {
                building.RemoveApartment(apartment);
            }

            foreach (var apartment in request.Model.Apartments)
            {
                if (apartment.IsNew)
                {
                    // Create new apartment
                    var apartmentId = EntityIDFactory.NextID<ApartmentID>();
                    var newApartment = new Apartment(apartmentId,
                        apartment.Code,
                        apartment.Dweller,
                        apartment.Owner,
                        apartment.SortingNum,
                        new Dictionary<Category.CategoryKind, double>()
                        {
                            {Category.CategoryKind.Heating, apartment.HeatingRatio * 0.001},
                            {Category.CategoryKind.Elevator, apartment.ElevatorRatio * 0.001},
                            {Category.CategoryKind.Shared, apartment.SharedRatio * 0.001},
                        });
                    building.AddApartment(newApartment);
                }
                else
                {
                    // Update existing
                    var apartmentId = EntityIDFactory.GetID<ApartmentID>(apartment.ID);
                    var existing = building.FindApartmentByID(apartmentId);
                    if (existing.RowVersion != apartment.RowVersion)
                        throw new ConcurrencyConflictException(existing, apartmentId, apartment.RowVersion);
                    existing.Code = apartment.Code;
                    existing.SetDweller(apartment.Dweller);
                    existing.SetOwner(apartment.Owner);
                    existing.SortingNum = apartment.SortingNum;
                    existing.Ratios = new Dictionary<Category.CategoryKind, double>()
                    {
                        {Category.CategoryKind.Heating, apartment.HeatingRatio * 0.001},
                        {Category.CategoryKind.Elevator, apartment.ElevatorRatio * 0.001},
                        {Category.CategoryKind.Shared, apartment.SharedRatio * 0.001},
                    };
                }
            }

            return Task.CompletedTask;
        }
    }
}
