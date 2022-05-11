using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Buildings
{
    public class BuildingID : EntityID
    {
        public BuildingID(long id) : base(id)
        {
        }

        public BuildingID() : base(0)
        {
        }
    }

    public class Building : Entity<BuildingID>
    {
        private readonly List<Apartment> _apartments = new List<Apartment>();

        public override BuildingID ID { get; protected set; }
        public string Description { get; set; }
        public Address Address { get; set; }

        public IReadOnlyCollection<Apartment> Apartments => _apartments.AsReadOnly();

        protected Building()
        {
        }

        public Building(BuildingID id) : this(id, Address.Empty(), Enumerable.Empty<Apartment>())
        {
        }

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
                           ?? throw new ArgumentException(
                               $"Apartment with ID {apartment.ID} does not belong to building!");
            _apartments.Remove(existing);
        }

        public Apartment FindApartmentByID(ApartmentID apartmentID)
        {
            return _apartments.SingleOrDefault(i => i.ID == apartmentID)
                   ?? throw new ArgumentException($"Apartment with ID {apartmentID} does not belong to building!");
        }


        public static Building ForTests() => new Building(
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
                        {Expenses.Category.CategoryKind.Heating, 0.0357},
                        {Expenses.Category.CategoryKind.Elevator, 0.00},
                        {Expenses.Category.CategoryKind.Shared, 0.0357}
                    }),
                new Apartment(
                    new ApartmentID(-2),
                    "ΙΣ2",
                    "Chumani",
                    "Chumani",
                    "Ισόγειο",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        {Expenses.Category.CategoryKind.Heating, 0.0845},
                        {Expenses.Category.CategoryKind.Elevator, 0.00},
                        {Expenses.Category.CategoryKind.Shared, 0.845}
                    }),
                new Apartment(
                    new ApartmentID(-3),
                    "Α",
                    "Μπράντζος",
                    "Μπράντζος",
                    "1ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        {Expenses.Category.CategoryKind.Heating, 0.2179},
                        {Expenses.Category.CategoryKind.Elevator, 0.0992},
                        {Expenses.Category.CategoryKind.Shared, 0.2179}
                    }),
                new Apartment(
                    new ApartmentID(-4),
                    "Β1",
                    "Αντωνίου",
                    "Αντωνίου",
                    "2ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        {Expenses.Category.CategoryKind.Heating, 0.1008},
                        {Expenses.Category.CategoryKind.Elevator, 0.0918},
                        {Expenses.Category.CategoryKind.Shared, 0.1008}
                    }),
                new Apartment(
                    new ApartmentID(-5),
                    "Β2",
                    "Βάκας",
                    "Βάκας",
                    "2ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        {Expenses.Category.CategoryKind.Heating, 0.0122},
                        {Expenses.Category.CategoryKind.Elevator, 0.0111},
                        {Expenses.Category.CategoryKind.Shared, 0.0122}
                    }),
                new Apartment(
                    new ApartmentID(-6),
                    "Γ1",
                    "Περσίδης",
                    "Περσίδης",
                    "3ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        {Expenses.Category.CategoryKind.Heating, 0.1008},
                        {Expenses.Category.CategoryKind.Elevator, 0.1377},
                        {Expenses.Category.CategoryKind.Shared, 0.1008}
                    }),
                new Apartment(
                    new ApartmentID(-7),
                    "Γ2",
                    "Κατσούλας",
                    "Κατσούλας",
                    "3ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        {Expenses.Category.CategoryKind.Heating, 0.1220},
                        {Expenses.Category.CategoryKind.Elevator, 0.1666},
                        {Expenses.Category.CategoryKind.Shared, 0.1220}
                    }),
                new Apartment(
                    new ApartmentID(-8),
                    "Δ",
                    "Πανσέληνος",
                    "Πανσέληνος",
                    "4ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        {Expenses.Category.CategoryKind.Heating, 0.1415},
                        {Expenses.Category.CategoryKind.Elevator, 0.2575},
                        {Expenses.Category.CategoryKind.Shared, 0.1415}
                    }),
                new Apartment(
                    new ApartmentID(-9),
                    "Ε",
                    "Πανσέληνος",
                    "Πανσέληνος",
                    "5ος",
                    new Dictionary<Expenses.Category.CategoryKind, double>
                    {
                        {Expenses.Category.CategoryKind.Heating, 0.0748},
                        {Expenses.Category.CategoryKind.Elevator, 0.1362},
                        {Expenses.Category.CategoryKind.Shared, 0.0748}
                    })
            })
        {
            ID          = new BuildingID(-1),
            Description = "Γιαννιτσών 23, Κυψέλη"
        };
    }
}
