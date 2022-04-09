using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Buildings
{
    public class BuildingID : EntityID
    {
        public BuildingID(long id) : base(id) { }
        public BuildingID() : base(0) { }
    }

    public class Building : Entity<BuildingID>
    {
        private readonly List<Apartment> _apartments = new List<Apartment>();

        public override BuildingID ID { get; protected set; }
        public string Description { get; set; }
        public Address Address { get; set; }

        public IReadOnlyCollection<Apartment> Apartments => _apartments.AsReadOnly();

        protected Building() { }

        public Building(BuildingID id, Address address, IEnumerable<Apartment> apartments) : base(id)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            foreach (var apartment in apartments)
                AddApartment(apartment);
        }

        public void AddApartment(Apartment apartment)
        {
            if (apartment.IsNew == false)
                if (_apartments.Find(a => a.ID == apartment.ID) != null)
                    throw new ArgumentException($"Apartment {apartment} already belongs to building!");

            _apartments.Add(apartment);
            apartment.SetBuilding(ID);
        }

        public void RemoveApartment(Apartment apartment)
        {
            var existing = _apartments.SingleOrDefault(i => i.ID == apartment.ID)
                ?? throw new ArgumentException($"Apartment with ID {apartment.ID} does not belong to building!");
            _apartments.Remove(existing);
        }


        public static Building ForTests() => new Building
        (
            new BuildingID(-1),
            new Address("Giannitson", "23", "Athens", "11363"),
            new List<Apartment>
            {
                new Apartment(
                    new ApartmentID(-1),
                    "ΙΣ1",
                    "Κοζάκης",
                    "Κοζάκης",
                    "Ισόγειο",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 35.70 },
                        { Expenses.Category.CategoryKind.Elevator, 0.00 },
                        { Expenses.Category.CategoryKind.Shared, 35.70 }
                    }),
                new Apartment(
                    new ApartmentID(-2),
                    "ΙΣ2",
                    "Chumani",
                    "Chumani",
                    "Ισόγειο",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 84.50 },
                        { Expenses.Category.CategoryKind.Elevator, 0.00 },
                        { Expenses.Category.CategoryKind.Shared, 84.50 }
                    }),
                new Apartment(
                    new ApartmentID(-3),
                    "Α",
                    "Μπράντζος",
                    "Μπράντζος",
                    "1ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 217.90 },
                        { Expenses.Category.CategoryKind.Elevator, 99.20 },
                        { Expenses.Category.CategoryKind.Shared, 217.90 }
                    }),
                new Apartment(
                    new ApartmentID(-4),
                    "Β1",
                    "Αντωνίου",
                    "Αντωνίου",
                    "2ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 100.80 },
                        { Expenses.Category.CategoryKind.Elevator, 91.80 },
                        { Expenses.Category.CategoryKind.Shared, 100.80 }
                    }),
                new Apartment(
                    new ApartmentID(-5),
                    "Β2",
                    "Βάκας",
                    "Βάκας",
                    "2ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 122.00 },
                        { Expenses.Category.CategoryKind.Elevator, 111.00 },
                        { Expenses.Category.CategoryKind.Shared, 122.00 }
                    }),
                new Apartment(
                    new ApartmentID(-6),
                    "Γ1",
                    "Περσίδης",
                    "Περσίδης",
                    "3ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 100.80 },
                        { Expenses.Category.CategoryKind.Elevator, 137.70 },
                        { Expenses.Category.CategoryKind.Shared, 100.80 }
                    }),
                new Apartment(
                    new ApartmentID(-7),
                    "Γ2",
                    "Κατσούλας",
                    "Κατσούλας",
                    "3ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 122.00 },
                        { Expenses.Category.CategoryKind.Elevator, 166.60 },
                        { Expenses.Category.CategoryKind.Shared, 122.00 }
                    }),
                new Apartment(
                    new ApartmentID(-8),
                    "Δ",
                    "Πανσέληνος",
                    "Πανσέληνος",
                    "4ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 141.50 },
                        { Expenses.Category.CategoryKind.Elevator, 257.50 },
                        { Expenses.Category.CategoryKind.Shared, 141.50 }
                    }),
                new Apartment(
                    new ApartmentID(-9),
                    "Ε",
                    "Πανσέληνος",
                    "Πανσέληνος",
                    "5ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        { Expenses.Category.CategoryKind.Heating, 74.80 },
                        { Expenses.Category.CategoryKind.Elevator, 136.20 },
                        { Expenses.Category.CategoryKind.Shared, 74.80 }
                    })
            })
        {
            ID          = new BuildingID(-1),
            Description = "Γιαννιτσών 23, Κυψέλη"
        };
    }
}