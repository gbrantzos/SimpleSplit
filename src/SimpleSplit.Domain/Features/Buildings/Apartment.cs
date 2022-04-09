using SimpleSplit.Common;
using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Buildings
{
    public class ApartmentID : EntityID
    {
        public ApartmentID() : base(0) { }

        public ApartmentID(long id) : base(id) { }
    }

    public class Apartment : Entity<ApartmentID>
    {
        public override ApartmentID ID { get; protected set; }
        public string Floor { get; set; }
        public string Code { get; set; }
        public string Owner { get; private set; }
        public string Dweller { get; private  set; }
        public BuildingID BuildingID { get; private set; }

        // Ratios per expense category. Ratio is between 0 and 1000
        public Dictionary<Expenses.Category.CategoryKind, double> Ratios { get; set; }

        public override string ToString() => $"{Code} - {Floor}, {Dweller ?? Owner}";
        
        public Apartment(ApartmentID id, string code, string dweller, string owner, string floor,
            Dictionary<Expenses.Category.CategoryKind, double> ratios) : base(id)
        {
            Code    = code    ?? throw new ArgumentNullException(nameof(code));
            Dweller = dweller ?? throw new ArgumentNullException(nameof(dweller));
            Owner   = owner   ?? dweller;
            Floor   = floor   ?? throw new ArgumentNullException(nameof(floor));
            Ratios  = ratios  ?? throw new ArgumentNullException(nameof(ratios));

            if (Ratios.Any(rt => rt.Value is < 0 or > 1000))
            {
                var ratiosStr = String.Join(" " ,ratios.Select(k => $"{k}").ToArray());
                throw new ArgumentException($"Invalid ratios values: {ratiosStr}");
            }

            var allCategoryKinds = Enum.GetValues<Expenses.Category.CategoryKind>().OrderBy(kind => kind);
            if (!Ratios.Keys.OrderBy(key => key).SequenceEqual(allCategoryKinds))
                throw new ArgumentException($"Not all expense kind ratios defined for apartment {Code}");
        }
        protected Apartment() { }

        public void SetBuilding(BuildingID buildingID)
        {
            BuildingID = buildingID.ThrowIfNull(nameof(buildingID));
        }

        public void SetOwner(string newOwner)
        {
            Owner = newOwner;
            // TODO Somehow keep history
        }
        
        public void SetDweller(string newDweller)
        {
            Dweller = newDweller;
            // TODO Somehow keep history
        }
    }
}