using SimpleSplit.Domain.Base;

namespace SimpleSplit.Domain.Features.Expenses
{
    public class CategoryID : EntityID
    {
        public CategoryID(long id) : base(id) { }
        public CategoryID() : base(0) { }
    }

    public class Category : Entity<CategoryID>
    {
        public enum CategoryKind
        {
            Heating = 1,
            Elevator = 2,
            Shared = 3
        }

        public override CategoryID ID { get; protected set; }
        public string Description { get; set; }
        public CategoryKind Kind { get; set; }
    }
}
